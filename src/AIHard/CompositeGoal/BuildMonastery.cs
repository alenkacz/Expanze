using System;
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

        public BuildMonastery(IMapController map)
            : base(map)
        {
            lastBestTown = null;
            lastBestPos = 0;
        }

        public override void Init()
        {
            if (lastBestTown == null)
                GetDesirability();

            AddSubgoal(new RaiseSources(map, PriceKind.BMonastery));
            AddSubgoal(new BuildMonasteryAtom(map, lastBestTown, lastBestPos));

            lastBestTown = null;
        }

        public override double GetDesirability()
        {
            List<ITown> towns = map.GetPlayerMe().GetTown();

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


            double hasMonasteryDesirability = (map.GetPlayerMe().GetBuildingCount(Building.Monastery) > 0) ? 0.01 : 1.0;
            double hasMoneyDesirability = Desirability.GetHasSources(PriceKind.BMonastery) / 4.0;
            double bestSourceDesirability = ((GetBestSource() - 40) / 60.0) / 2.0;
            if (bestSourceDesirability > 0.5)
                bestSourceDesirability = 0.5;
            double desirability = (bestDesirability / 4.0 + hasMoneyDesirability + bestSourceDesirability) * hasMonasteryDesirability;

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
