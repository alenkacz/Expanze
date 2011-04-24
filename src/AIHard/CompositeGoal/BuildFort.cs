using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildFort : CompositeGoal
    {
        ITown lastBestTown;
        byte lastBestPos;

        public BuildFort(IMapController map, int depth) : base(map, depth, "Build fort")
        {
           lastBestTown = null;
            lastBestPos = 0;
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
            List<ITown> towns = map.GetPlayerMe().GetTown();

            if (map.GetTurnNumber() < 4)
                return 0.0f;

            double bestDesirability = 0.0;
            double tempFitness;
            lastBestTown = null;

            foreach (ITown town in towns)
            {
                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    if (town.GetBuildingKind(loop1) != BuildingKind.NoBuilding)
                        continue;

                    tempFitness = GetDesirability(town, loop1);
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


            double hasFortDesirability = (map.GetPlayerMe().GetBuildingCount(Building.Fort) > 0) ? 0.01 : 1.0;
            double hasMoneyDesirability = Desirability.GetHasSources(PriceKind.BFort);
            double pointsToWinDesirability = (map.GetPlayerMe().GetPoints() / (double)map.GetGameSettings().GetWinningPoints());
            double hasSomeoneFort = (Desirability.HasSomeoneBuilding(Building.Fort)) ? 0.0 : 1.0;
            double desirability = (bestDesirability * 3.0 / 8.0 + hasMoneyDesirability / 8.0 + pointsToWinDesirability / 4.0 + hasSomeoneFort / 4.0) * hasFortDesirability;

            return desirability;
        }

        private double GetDesirability(ITown town, byte pos)
        {
            IHexa hexa = town.GetIHexa(pos);
            HexaKind kind = hexa.GetKind();
            if (kind == HexaKind.Mountains ||
               kind == HexaKind.Water)
                return 0.0;


            int startSource = hexa.GetStartSource();

            double desirability = 1 - startSource / 24.0;

            return desirability;
        }
    }
}
