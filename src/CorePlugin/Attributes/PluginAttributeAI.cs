using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttributeAI : Attribute
    {
        public PluginAttributeAI(string description)
        {
            m_description = description;
        }

        private string m_description;

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }
    }
}
