using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using CorePlugin.Attributes;

namespace AIHard
{
    [PluginAttributeAI("Goal driven AI")]
    class AIHard : IComponentAI
    {
        IMapController mapController;
        ThinkGoal thinkGoal;

        public String GetAIName()
        {
            return "AI těžká";
        }

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;
            Desirability.SetMapController(mapController);
            thinkGoal = new ThinkGoal(mapController);
        }

        public void ResolveAI()
        {
            thinkGoal.Init();
            while (thinkGoal.Process() != GoalState.Active)
                ;
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIHard();
            return component;
        }

        public static int SumVector(int[] a)
        {
            int sum = 0;
            for (int loop1 = 0; loop1 < a.Length; loop1++)
                sum += a[loop1];
            return sum;
        }
        public static int[] Sum2Vectors(int[] a, int[] b)
        {
            int[] c = new int[a.Length];

            for (int loop1 = 0; loop1 < c.Length; loop1++)
                c[loop1] = a[loop1] + b[loop1];

            return c;
        }
    }
}
