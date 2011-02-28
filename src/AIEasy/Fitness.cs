using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    static class Fitness
    {
        static IMapController map;
        static AIEasy ai;

        public static void SetMapController(IMapController mapController, AIEasy aiEasy) 
        {
            map = mapController;
            ai = aiEasy;
        }

        public static float GetFitness(Object o)
        {
            return 0.0f;
        }

        public static float GetFitness(IRoad road)
        {
            if (road == null)
                return 0.0f;

            float fitness = 0.0f;
            IRoad tempRoad;
            foreach (ITown town1 in road.GetITown())
            {
                fitness += GetFitness(town1);

                for(byte loop1 = 0; loop1 < 3; loop1++)
                {
                    tempRoad = town1.GetIRoad(loop1);
                    foreach(ITown town2 in tempRoad.GetITown())
                    {
                        fitness += GetFitness(town2) / 2.0f;
                    }
                }
            }

            return fitness / 2.0f;
        }

        public static float GetFitness(ITown town)
        {
            if (town == null)
                return 0.0f;

            if(!town.IsPossibleToBuildTown())
                return 0.0f;

            int amountSources = 0;

            IHexa tempHexa;
            for (byte loop1 = 0; loop1 < 3; loop1++)
            {
                tempHexa = town.GetIHexa(loop1);
                if (tempHexa != null)
                {
                    amountSources += tempHexa.GetStartSource();
                }
            }

            return amountSources / 72.0f;
        }
    }
}
