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
        double kPoints;
        double kHasOtherFort;
        double kHasFort;

        public BuildFort(IMapController map, double kBestHexa, double kHasMoney, double kPoints, double kHasOtherFort, int depth) : base(map, depth, "Build fort")
        {
            this.kHasFort = 0.2;

            double sum = kBestHexa + kHasMoney + kPoints + kHasOtherFort;
            this.kBestHexa = kBestHexa / sum;
            this.kHasMoney = kHasMoney / sum;
            this.kPoints = kPoints / sum;
            this.kHasOtherFort = kHasOtherFort / sum;

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


            double hasFortDesirability = (map.GetPlayerMe().GetBuildingCount(Building.Fort) > 0 && map.GetActionPoints(PlayerPoints.Fort) == 0) ? kHasFort : 1.0;

            double hasMoneyDesirability = Desirability.GetHasSources(PriceKind.BFort);
            double points = (map.GetActionPoints(PlayerPoints.Fort) + map.GetActionPoints(PlayerPoints.FortParade)) / 2.0f;
            if (points > 1.0f)
                points = 1.0f;
            double hasSomeoneFort = (Desirability.HasSomeoneBuilding(Building.Fort)) ? 0.0 : 1.0;
            double desirability = (bestDesirability * kBestHexa + hasMoneyDesirability * kHasMoney + points * kPoints + hasSomeoneFort * kHasOtherFort) * hasFortDesirability;

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
