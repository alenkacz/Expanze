using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildMarket : CompositeGoal
    {
        ITown lastBestTown;
        byte lastBestPos;

        double kHexa; 
        double kHasSources;
        double kBestSource;
        double kHasOtherMarket;
        double kHasMarket;

        public BuildMarket(IMapController map, double k, double kHasMarket, int depth)
            : base(map, depth, "BuildMarket")
        {
            const double koef = 1000.0;

            k *= koef;
            kHasSources = k;
            k = (k - (int)kHasSources) * koef;
            kBestSource = k;
            k = (k - (int)kBestSource) * koef;
            kHexa = k;
            k = (k - (int)kHexa) * koef;
            kHasOtherMarket = k;

            BaseInit(kHasMarket);
        }

        private void BaseInit(double kHasMarket)
        {
            this.kHasMarket = kHasMarket;

            double sum = kHexa + kHasSources + kBestSource + kHasOtherMarket;

            this.kHexa = kHexa / sum;
            this.kHasSources = kHasSources / sum;
            this.kBestSource = kBestSource / sum;
            this.kHasOtherMarket = kHasOtherMarket / sum;

            lastBestTown = null;
            lastBestPos = 0;
        }

        public BuildMarket(IMapController map, double kHexa, double kHasSources, double kBestSource, double kHasOtherMarket, double kHasMarket, int depth)
            : base(map, depth, "BuildMarket")
        {
            BaseInit(kHasMarket);
        }

        public override void Init()
        {
            if (lastBestTown == null)
                GetDesirability();

            AddSubgoal(new RaiseSources(map, PriceKind.BMarket, depth + 1));
            AddSubgoal(new BuildMarketAtom(map, lastBestTown, lastBestPos, depth + 1));
            
            lastBestTown = null;
        }

        public override double GetDesirability()
        {
            List<ITown> towns = map.GetPlayerMe().GetTown();

            if(map.IsBanAction(PlayerAction.BuildMarket))
                return 0.0f;

            if (map.GetTurnNumber() < 3)
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
            if (HasFreeSlot())
                return 0.0;


            double hasMarketDesirability = (map.GetPlayerMe().GetBuildingCount(Building.Market) > 0) ? kHasMarket : 1.0;
            double hasMoneyDesirability = Desirability.GetHasSources(PriceKind.BMarket);
            double bestSourceDesirability = ((GetBestSource() - 40) / 60.0);
            double hasSomeoneMarket = (Desirability.HasSomeoneBuilding(Building.Market)) ? 0.0 : 1.0;
            if(bestSourceDesirability > 1.0)
                bestSourceDesirability = 1.0;
            double desirability = (bestDesirability * kHexa + hasMoneyDesirability * kHasSources + bestSourceDesirability * kBestSource + hasSomeoneMarket * kHasOtherMarket) * hasMarketDesirability;

            return desirability;
        }

        private int GetBestSource()
        {
            IPlayer me = map.GetPlayerMe();
            ISourceAll source = me.GetCollectSourcesNormal();
            int max = 0;
            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (source[loop1] > max &&
                    me.GetMarketLicence((SourceKind)loop1) != LicenceKind.SecondLicence)
                {
                    max = source[loop1];
                }
            }
            return max;
        }

        private bool HasFreeSlot()
        {
            List<IMarket> marketList = map.GetPlayerMe().GetMarket();
            foreach (IMarket market in marketList)
            {
                if (market.GetFreeSlot() > 0)
                    return true;
            }

            return false;
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
