using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildTown : CompositeGoal
    {
        ITown lastBestTown;
        List<IRoad> lastBestRoads;

        public BuildTown(IMapController map)
            : base(map)
        {
            lastBestTown = null;
            lastBestRoads = null;
        }

        public override void Init()
        {
            switch (map.GetState())
            {
                case EGameState.StateFirstTown:
                case EGameState.StateSecondTown:
                    FirstTwoStates();
                    if (lastBestTown != null)
                    {
                        AddSubgoal(new BuildTownAtom(map, lastBestTown));
                        lastBestTown = null;
                    }
                    break;

                case EGameState.StateGame:
                    if (lastBestTown != null)
                    {
                        List<IRoad> path = map.GetRoadsToTown(lastBestTown, map.GetPlayerMe());
                        foreach (IRoad road in path)
                        {
                            AddSubgoal(new BuildRoadAtom(map, road));
                        }
                        AddSubgoal(new BuildTownAtom(map, lastBestTown));
                        lastBestTown = null;
                    }
                    break;
            }
        }

        
        public override GoalState Process()
        {
            GoalState state = base.Process();          

            return state;
        }

        private void FirstTwoStates()
        {
            int townMaxID = map.GetMaxTownID();
            ITown tempTown;
            double tempFitness;
            ITown bestTown = null;
            double bestFitness = 0.0;

            for (int loop1 = 1; loop1 < townMaxID; loop1++)
            {
                tempTown = map.GetITownByID(loop1);
                tempFitness = GetFitness(tempTown);
                if (tempFitness > bestFitness)
                {
                    bestFitness = tempFitness;
                    bestTown = tempTown;
                }
            }

            lastBestTown = bestTown;
        }

        public override double GetFitness()
        {
            double bestFitness = 0.0f;
            double tempFitness;
            ITown tempTown;
            lastBestTown = null;

            if (map.GetState() != EGameState.StateGame)
                return 2.0f;
            else
            {
                int maxTownID = map.GetMaxTownID();
                for (int loop1 = 1; loop1 < maxTownID; loop1++)
                {
                    tempTown = map.GetITownByID(loop1);
                    tempFitness = GetFitness(tempTown);

                    /// it is not possible to build town on that place
                    if (tempFitness < 0.01)
                        continue;

                    //List<IRoad> roads = map.GetRoadsToTown(tempTown, map.GetPlayerMe());
                    int dst = map.GetDistanceToTown(tempTown, map.GetPlayerMe());
                    //map.Log("ahoj.txt", dst + "");
                    double coef = (dst <= 2) ? 1 - (dst * 0.1) : 1 - (dst * 0.15);
                    tempFitness = tempFitness * coef;

                    if (tempFitness > bestFitness)
                    {
                        bestFitness = tempFitness;
                        lastBestTown = tempTown;
                        //lastBestRoads = roads;
                    }
                }
            }

            return bestFitness;
        }

        private double GetFitness(IHexa hexa)
        {
            if (hexa == null)
                return 0.0f;

            double fitness;

            fitness = hexa.GetStartSource() / 24.0;

            /*
             * Ore and stone is more useful than other sources 
             */
            if (hexa.GetKind() != HexaKind.Mountains &&
               hexa.GetKind() != HexaKind.Stone)
            {
                fitness *= 0.85;
            }

            /*
             * Better than water
             */

            if (hexa.GetKind() == HexaKind.Desert)
                fitness = 0.15;

            return fitness;
        }

        private double GetFitness(ITown town)
        {
            if (town == null)
                return 0.0;

            if (!town.IsPossibleToBuildTown())
                return 0.0;

            double fitness = 0.0;
            double multiplier = 1.0;
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
                    multiplier = 3.5;
                }
                else
                    multiplier = 1.0;

                fitness += GetFitness(tempHexa) * multiplier / 9.0;
            }

            return fitness;
        }
    }
}
