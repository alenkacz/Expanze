﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildMonastery : CompositeGoal
    {
        ITown lastBestTown;
        byte lastBestPos;

        double kHexa; 
        double kHasSources;
        double kBestSource;
        double kHasOtherMonastery;
        double kHasMonastery;

         public BuildMonastery(IMapController map, double k, double kHasMonastery, int depth)
            : base(map, depth, "Build Monastery")
        {
            const double koef = 1000.0;

            k *= koef;
            kHasSources = k;
            k = (k - (int)kHasSources) * koef;
            kBestSource = k;
            k = (k - (int)kBestSource) * koef;
            kHexa = k;
            k = (k - (int)kHexa) * koef;
            kHasOtherMonastery = k;

            BaseInit(kHasMonastery);
        }

        private void BaseInit(double kHasMonastery)
        {
            this.kHasMonastery = kHasMonastery;

            double sum = kHexa + kHasSources + kBestSource + kHasOtherMonastery;

            this.kHexa = kHexa / sum;
            this.kHasSources = kHasSources / sum;
            this.kBestSource = kBestSource / sum;
            this.kHasOtherMonastery = kHasOtherMonastery / sum;

            lastBestTown = null;
            lastBestPos = 0;
        }

        public BuildMonastery(IMapController map, double kHexa, double kHasSources, double kBestSource, double kHasOtherMonastery, double kHasMonastery, int depth)
            : base(map, depth, "Build Monastery")
        {
            BaseInit(kHasMonastery);
        }

        public override void Init()
        {
            if (lastBestTown == null)
                GetDesirability();

            AddSubgoal(new RaiseSources(map, PriceKind.BMonastery, depth + 1));
            AddSubgoal(new BuildMonasteryAtom(map, lastBestTown, lastBestPos, depth + 1));

            lastBestTown = null;
        }

        public override double GetDesirability()
        {
            List<ITown> towns = map.GetPlayerMe().GetTown();

            if(map.IsBanAction(PlayerAction.BuildMonastery))
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


            double hasMonasteryDesirability = (map.GetPlayerMe().GetBuildingCount(Building.Monastery) > 0) ? kHasMonastery : 1.0;
            double hasMoneyDesirability = Desirability.GetHasSources(PriceKind.BMonastery);
            double bestSourceDesirability = ((GetBestSource() - 40) / 60.0);
            double hasSomeoneMonastery = (Desirability.HasSomeoneBuilding(Building.Monastery)) ? 0.0 : 1.0;
            if (bestSourceDesirability > 1.0)
                bestSourceDesirability = 1.0;
            double desirability = (bestDesirability * kHexa + hasMoneyDesirability * kHasSources + bestSourceDesirability * kBestSource + hasSomeoneMonastery * kHasOtherMonastery) * hasMonasteryDesirability;

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
                    me.GetMonasteryUpgrade((SourceBuildingKind)loop1) != UpgradeKind.SecondUpgrade)
                {
                    max = source[loop1];
                }
            }
            return max;
        }

        private bool HasFreeSlot()
        {
            List<IMonastery> monasteryList = map.GetPlayerMe().GetMonastery();
            foreach (IMonastery monastery in monasteryList)
            {
                if (monastery.GetFreeSlot() > 0)
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
