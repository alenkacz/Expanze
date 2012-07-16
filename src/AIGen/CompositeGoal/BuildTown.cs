using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class TownPlayer
    {
        public ITown town;
        public bool me;
        public int distance;

        public TownPlayer(ITown town, bool me, int distance)
        {
            this.town = town;
            this.me = me;
            this.distance = distance;
        }
    }

    class BuildTown : CompositeGoal
    {
        ITown lastBestTown;

        double kNearestTown;
        double kTownItself;
        double kPoints;
        BuildSourceBuilding buildingGoal;

        public BuildTown(IMapController map, double kNearestTown, double kTownItself, double kPoints, int depth, BuildSourceBuilding buildingGoal)
            : base(map, depth, "Build Town")
        {
            lastBestTown = null;

            double sum = (kNearestTown + kTownItself + kPoints);
            this.kNearestTown = kNearestTown / sum;
            this.kTownItself = kTownItself / sum;
            this.kPoints = kPoints / sum;
            this.buildingGoal = buildingGoal;
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
                        AddSubgoal(new BuildTownAtom(map, lastBestTown, depth + 1));
                        lastBestTown = null;
                    }
                    break;

                case EGameState.StateGame:
                    if (lastBestTown != null)
                    {
                        List<IRoad> path = map.GetRoadsToTown(lastBestTown, map.GetPlayerMe());

                        

                        List<ISourceAll> sourceList = new List<ISourceAll>();
                        sourceList.Add(map.GetPrice(PriceKind.BTown));

                        for (int loop1 = 0; loop1 < path.Count; loop1++)
                        {
                            sourceList.Add(map.GetPrice(PriceKind.BRoad));
                        }

                        AddSubgoal(new RaiseSources(map, sourceList, depth + 1));

                        for (int loop1 = 0; loop1 < path.Count; loop1++)
                        {
                            AddSubgoal(new BuildRoadAtom(map, path[loop1], depth + 1));
                        }

                        AddSubgoal(new BuildTownAtom(map, lastBestTown, depth + 1));

                        lastBestTown = null;
                    }
                    break;
            }
        }

        
        public override GoalState Process()
        {
            GoalState state = base.Process();

            if (state == GoalState.Active &&
                map.GetState() != EGameState.StateGame)
                subgoals.Clear();

            return state;
        }

        private void FirstTwoStates()
        {
            int townMaxID = map.GetMaxTownID();
            ITown tempTown;
            double tempDesirability;
            ITown bestTown = null;
            double bestDesirability = 0.0;
            double count = 0;
            double maxCount = 0;

            for (int loop1 = 1; loop1 < townMaxID; loop1++)
            {
                tempTown = map.GetITownByID(loop1);
                count = CountNearestTowns(tempTown);
                if (count > maxCount)
                    maxCount = count;
            }

            for (int loop1 = 1; loop1 < townMaxID; loop1++)
            {
                tempTown = map.GetITownByID(loop1);
                tempDesirability = GetDesirability(tempTown);
                
                if(tempDesirability > 0.1)
                {
                    count = CountNearestTowns(tempTown);
                    
                    /*
                    map.Log("nearest", "" + count + " townID -" + tempTown.GetTownID() + " hexas -" +
                        tempTown.GetIHexa(0).GetKind() + tempTown.GetIHexa(0).GetStartSource() + "  :  " +
                        tempTown.GetIHexa(1).GetKind() + tempTown.GetIHexa(1).GetStartSource() + "  :  " +
                        tempTown.GetIHexa(2).GetKind() + tempTown.GetIHexa(2).GetStartSource() + "  :  ");
                    */
                }

                tempDesirability = tempDesirability * kTownItself + (count / (double) maxCount) * kNearestTown;

                if (tempDesirability > bestDesirability)
                {
                    bestDesirability = tempDesirability;
                    bestTown = tempTown;
                }
            }

            lastBestTown = bestTown;
        }

        private double CountNearestTowns(ITown futureTown)
        {
            Queue<TownPlayer> openList = new Queue<TownPlayer>();

            foreach (IPlayer player in map.GetPlayerOthers())
            {
                foreach (ITown town in player.GetTown())
                {
                    openList.Enqueue(new TownPlayer(town, false, 0));
                }
            }

            foreach (ITown town in map.GetPlayerMe().GetTown())
            {
                openList.Enqueue(new TownPlayer(town, false, 0));
            }
            openList.Enqueue(new TownPlayer(futureTown, true, 0));

            TownPlayer peekTown;
            ITown neighbour;
            List<ITown> closeList = new List<ITown>();

            double count = 0.0;

            while (openList.Count > 0)
            {
                peekTown = openList.Dequeue();
                closeList.Add(peekTown.town);
                if (peekTown.me && peekTown.town.IsPossibleToBuildTown())
                {
                    bool isNeighbour = false;

                    for(byte loop1 = 0; loop1 < 3; loop1++)
                    {
                        if (peekTown.town.GetITown(loop1) != null &&
                            peekTown.town.GetITown(loop1).GetTownID() == futureTown.GetTownID())
                        {
                            isNeighbour = true;
                            break;
                        }
                    }

                    if (!isNeighbour)
                    {
                        double townDesirability = GetDesirability(peekTown.town) / (peekTown.distance + 1);
                        count += townDesirability;
                    }
                }

                /*
                 * Expand 
                 */

                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    neighbour = peekTown.town.GetITown(loop1);

                    if (neighbour != null &&
                        !closeList.Contains(neighbour))
                    {
                        bool isInOpenList = false;
                        foreach (TownPlayer tp in openList)
                        {
                            if (tp.town.GetTownID() == neighbour.GetTownID())
                            {
                                isInOpenList = true;
                                break;
                            }
                        }

                        if (!isInOpenList)
                        {
                            openList.Enqueue(new TownPlayer(neighbour, peekTown.me, peekTown.distance + 1));
                        }
                    }
                }
            }

            return count;
        }

        public override double GetDesirability()
        {
            double bestFitness = 0.0f;
            double tempDesirability;
            ITown tempTown;
            lastBestTown = null;

            if (map.GetState() != EGameState.StateGame)
                return Double.MaxValue;
            else
            {
                int maxTownID = map.GetMaxTownID();
                double count = 0;
                double maxCount = 0;

                for (int loop1 = 1; loop1 < maxTownID; loop1++)
                {
                    tempTown = map.GetITownByID(loop1);
                    count = CountNearestTowns(tempTown);
                    if (count > maxCount)
                        maxCount = count;
                }

                for (int loop1 = 1; loop1 < maxTownID; loop1++)
                {
                    tempTown = map.GetITownByID(loop1);
                    tempDesirability = GetDesirability(tempTown);

                    /// it is not possible to build town on that place
                    if (tempDesirability < 0.01)
                        continue;

                    int dst = map.GetDistanceToTown(tempTown, map.GetPlayerMe());
                    if (dst > 20)
                        continue;

                    count = CountNearestTowns(tempTown);

                    int points = map.GetActionPoints(PlayerPoints.Town);
                    tempDesirability = tempDesirability * kTownItself + (count / (double)maxCount) * kNearestTown +
                        ((points > 0) ? 1.0f : 0.0f) * kPoints;

                    double coef = (dst <= 2) ? 1 - (dst * 0.1) : 1 - (dst * 0.15);
                    tempDesirability = tempDesirability * coef;

                    if (tempDesirability > bestFitness)
                    {
                        bestFitness = tempDesirability;
                        lastBestTown = tempTown;
                    }
                }
            }

            return bestFitness;
        }

        private double GetFitness(IHexa hexa)
        {
            if (hexa == null || hexa.GetKind() == HexaKind.Water)
                return 0.0f;

            return buildingGoal.GetDesirability(hexa);
        }
        /*private double GetFitness(IHexa hexa)
        {
            if (hexa == null || hexa.GetKind() == HexaKind.Water)
                return 0.0f;

            double fitness;

            fitness = hexa.GetStartSource() / 24.0;

            if (hexa.GetKind() != HexaKind.Mountains &&
               hexa.GetKind() != HexaKind.Stone)
            {
                if (hexa.GetKind() == HexaKind.Forest)
                {
                    fitness *= 0.6;
                }
                else
                    fitness *= 0.75;
            }
            else
                fitness *= 1.2;

            if (hexa.GetKind() == HexaKind.Desert)
                fitness = 0.15;

            return fitness;
        }*/

        private double GetDesirability(ITown town)
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

                if (tempHexa.GetStartSource() == 0 &&
                    tempHexa.GetKind() != HexaKind.Desert)
                    continue;

                if (tempHexa.GetKind() != HexaKind.Desert && 
                    !haveKind[normal.KindToInt(tempHexa.GetSourceKind())])
                {
                    haveKind[normal.KindToInt(tempHexa.GetSourceKind())] = true;
                    multiplier = 2.5;
                }
                else
                    multiplier = 1.0;

                double fitnessHexa = GetFitness(tempHexa);
                if (fitnessHexa >= ThinkGoal.ONE_POINT_REMAIN_FITNESS - 1.0f)
                    fitnessHexa /= 90.0; // dont build town if you can build building for points
                fitness += fitnessHexa * multiplier / 9.0;
            }

            double desirabilityWinningBonus = 0.0;

            //if (map.GetPlayerMe().GetPoints()[(int) PlayerPoints.Town] < map.GetActionPoints(PlayerPoints.Town))
            //    desirabilityWinningBonus = ThinkGoal.ONE_POINT_REMAIN_FITNESS;
            return fitness + desirabilityWinningBonus;
        }
    }
}
