/*
 * I have used :
 * Introduction to Building a Plug-In Architecture Using C#
 * Matthew Cochran
 * September 10, 2007 
 * http://www.c-sharpcorner.com/uploadfile/rmcochran/plug_in_architecture09092007111353am/plug_in_architecture.aspx
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using CorePlugin.Attributes;

namespace CorePlugin
{
    public static class CoreProviderAI
    {
        private static List<IComponentAI> m_ai;

        public static List<IComponentAI> AI
        {
            get
            {
                if (null == m_ai)
                    Reload();

                return m_ai;
            }
        }


        public static void Reload()
        {
            if (null == m_ai)
                m_ai = new List<IComponentAI>();
            else
                m_ai.Clear();

            //m_ai.Add(new CalculatorHost()); // load the default

            List<Assembly> plugInAssemblies = LoadPlugInAssemblies();
            List<IComponentAI> plugIns = GetPlugIns(plugInAssemblies);

            foreach (IComponentAI pluginAI in plugIns)
            {
                m_ai.Add(pluginAI);
            }
        }

        private static List<Assembly> LoadPlugInAssemblies()
        {
            DirectoryInfo dInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Plugins"));
            FileInfo[] files = dInfo.GetFiles("*.dll");

            List<Assembly> plugInAssemblyList = new List<Assembly>();

            if (null != files)
            {
                foreach (FileInfo file in files)
                {
                    plugInAssemblyList.Add(Assembly.LoadFile(file.FullName));
                }
            }
            return plugInAssemblyList;
        }

        static List<IComponentAI> GetPlugIns(List<Assembly> assemblies)
        {
            List<Type> availableTypes = new List<Type>();

            foreach (Assembly currentAssembly in assemblies)
                availableTypes.AddRange(currentAssembly.GetTypes());

            // get a list of objects that implement the IComponentAI interface AND 
            // have the CalculationPlugInAttribute
            List<Type> calculatorList = availableTypes.FindAll(delegate(Type t)
            {
                List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
                object[] arr = t.GetCustomAttributes(typeof(PluginAttributeAI), true);
                return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(IComponentAI));
            });

            // conver the list of Objects to an instantiated list of ICalculators
            return calculatorList.ConvertAll<IComponentAI>(delegate(Type t) { return Activator.CreateInstance(t) as IComponentAI; });
        }
    }
}
