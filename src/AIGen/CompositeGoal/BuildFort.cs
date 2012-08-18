using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class BuildFort : CompositeGoal
    {
        ITown lastBestTown;
        byte lastBestPos;

        double kBestHexa;
        double kHasMoney;
        double kCapture;
        double kHasOtherFort;
        double kHasFort;

        BuildTown buildTown;

        public BuildFort(IMapController map, double kBestHexa, double kHasMoney, double kCapture, double kHasOtherFort, int depth, BuildTown buildTown) : base(map, depth, "Build fort")
        {
            this.kHasFort = 0.2;

            double sum = kBestHexa + kHasMoney + kCapture + kHasOtherFort;
            this.kBestHexa = kBestHexa / sum;
            this.kHasMoney = kHasMoney / sum;
            this.kCapture = kCapture / sum;
            this.kHasOtherFort = kHasOtherFort / sum;

            lastBestTown = null;
            lastBestPos = 0;

            this.buildTown = buildTown;
        }

        public override void Init()
        {
            if (lastBestTown == null)
                GetDesirability();

            AddSubgoal(new RaiseSources(map, PriceKind.BFort, depth + 1));
            AddSubgoal(new BuildFortAtom(map, lastBestTown, lastBestPos, depth + 1));
            
            lastBestTown = null;
        }

        public override double GetDesirability()
        {
            if (map.GetPlayerMe().GetCollectSourcesNormal().GetAsArray().Sum() < 70)
                return 0.0;

            List<ITown> towns = map.GetPlayerMe().GetTown();

            double bestDesirability = 0.0;
            double tempFitness;
            lastBestTown = null;

            foreach (ITown town in towns)
            {
                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    if (town.GetBuildingKind(loop1) != BuildingKind.NoBuilding ||
                        town.GetIHexa(loop1).GetKind() == HexaKind.Mountains ||
                        town.GetIHexa(loop1).GetKind() == HexaKind.Water ||
                        town.GetIHexa(loop1).GetKind() == HexaKind.Null)
                        continue;

                    tempFitness = GetDesirability(town, loop1) * kBestHexa + GetCaptureAndBuildTownDesirability(town, loop1) * kCapture;
                    if (tempFitness > bestDesirability)
                    {
                        lastBestTown = town;
                        lastBestPos = loop1;
                        bestDesirability = tempFitness;
                    }
                }
            }

            if (bestDesirability == 0.0)
                return bestDesirability;


            double hasFortDesirability = (map.GetPlayerMe().GetBuildingCount(Building.Fort) > 0 && map.GetActionPoints(PlayerPoints.Fort) == 0) ? kHasFort : 1.0;

            double hasMoneyDesirability = Desirability.GetHasSources(PriceKind.BFort);
            //double points = (map.GetActionPoints(PlayerPoints.Fort) + map.GetActionPoints(PlayerPoints.FortParade)) / 2.0f;
            //if (points > 1.0f)
            //    points = 1.0f;
            double hasSomeoneFort = (Desirability.HasSomeoneBuilding(Building.Fort)) ? 0.0 : 1.0;
            double desirability = bestDesirability + hasMoneyDesirability * kHasMoney + hasSomeoneFort * kHasOtherFort;

            double desirabilityWinningBonus = 0.0;
            //if (map.GetPlayerMe().GetPoints()[(int) PlayerPoints.Fort] < map.GetActionPoints(PlayerPoints.Fort))
            //    desirabilityWinningBonus = ThinkGoal.ONE_POINT_REMAIN_FITNESS;
            return desirability + desirabilityWinningBonus;
        }

        private double GetCaptureAndBuildTownDesirability(ITown town, byte pos)
        {
            IHexa hexa = town.GetIHexa(pos);

            if (map.IsBanAction(PlayerAction.FortCaptureHexa))
            {
                if (hexa.GetKind() == HexaKind.Mountains)
                    return 0.0;

                return 1.0;
            }

            double desirability = 0.0;
            for(int loop1 = 0; loop1 < 6; loop1++)
            {
                IHexa neigbour = hexa.GetIHexaNeighbour((RoadPos) loop1);
                if (neigbour != null && neigbour.GetKind() == HexaKind.Water)
                {
                    for (int loop2 = 0; loop2 < 6; loop2++)
                    {
                        IHexa neigbour2 = neigbour.GetIHexaNeighbour((RoadPos)loop1);
                        
                        if (neigbour2 != null && neigbour2.GetKind() != HexaKind.Water && !map.IsInFortRadius(neigbour2, map.GetPlayerMe()))
                        {
                            ITown bestTown = null;
                            for(int loop3 = 0; loop3 < 6; loop3++)
                            {
                                if(buildTown.GetDesirability(neigbour2.GetITown((TownPos) loop3)) > 0.3)
                                    bestTown = neigbour2.GetITown((TownPos) loop3);
                            }
                            if(bestTown != null)
                            {
                                int dist = map.GetDistance(town, bestTown);
                                if(dist > 20)
                                    desirability += 1.0;
                            }
                        }
                    }
                }
            }

            return desirability;
        }

        private double GetDesirability(ITown town, byte pos)
        {
            IHexa hexa = town.GetIHexa(pos);
            HexaKind kind = hexa.GetKind();
            if (kind == HexaKind.Mountains ||
               kind == HexaKind.Water)
                return 0.0;

            foreach (IFort fort in map.GetPlayerMe().GetFort())
            {
                if (fort.GetHexaID() == hexa.GetID())
                    return 0.0;
            }

            int startSource = hexa.GetStartSource();

            double desirability = 1 - startSource / 24.0;  

            return desirability;
        }
    }
}
