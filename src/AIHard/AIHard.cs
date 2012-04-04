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

        public void InitAIComponent(IMapController mapController, double[] koef)
        {
            this.mapController = mapController;
            Desirability.SetMapController(mapController);
            koef = new double[] { 0.50753910, 0.69240258, 0.75383028, 0.005387260, 0.03888009, 0.31205954, 0.73031067, 0.38760135, 0.33115000, 0.85088233, 0.55350950, 0.39943568, 0.18883969, 0.41989920, 0.20405476, 0.34062831, 0.33398948, 0.71093270, 0.69218646 };
            thinkGoal = new ThinkGoal(mapController, koef, 0);
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
