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

        public static float GetFitness(IHexa hexa)
        {
            if (hexa == null)
                return 0.0f;

            float fitness;

            fitness = hexa.GetStartSource() / 24.0f;

            return fitness;
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
                    if (tempRoad == null)
                        break;
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

            float fitness = 0.0f;
            float multiplier = 1.0f;
            ISourceAll normal = map.GetPlayerMe().GetCollectSourcesNormal();
            bool[] haveKind = new bool[5];
            for (int loop1 = 0; loop1 < 5; loop1++)
                haveKind[loop1] = normal[loop1] != 0;

            IHexa tempHexa;
            for (byte loop1 = 0; loop1 < 3; loop1++)
            {
                tempHexa = town.GetIHexa(loop1);

                if (tempHexa.GetStartSource() == 0)
                    continue;

                if (!haveKind[normal.KindToInt(tempHexa.GetSourceKind())])
                {
                    haveKind[normal.KindToInt(tempHexa.GetSourceKind())] = true;
                    multiplier = 3.0f;
                }
                else
                    multiplier = 1.0f;

                fitness += Fitness.GetFitness(tempHexa) * multiplier / 9.0f;
            }

            return fitness;
        }
    }
}
