using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class InventUpgrade : CompositeGoal
    {
        SourceBuildingKind bestKind;

        public InventUpgrade(IMapController map, int depth)
            : base(map, depth, "Invent upgrade")
        {
        }

        public override void Init()
        {
            if (bestKind != SourceBuildingKind.Count)
            {
                UpgradeKind upgrade = UpgradeKind.NoUpgrade;
                switch (map.GetPlayerMe().GetMonasteryUpgrade(bestKind))
                {
                    case UpgradeKind.NoUpgrade: upgrade = UpgradeKind.FirstUpgrade; break;
                    case UpgradeKind.FirstUpgrade: upgrade = UpgradeKind.SecondUpgrade; break;
                    case UpgradeKind.SecondUpgrade: return;
                }

                AddSubgoal(new RaiseSources(map, map.GetPrice(bestKind, upgrade), depth));
                AddSubgoal(new InventUpgradeAtom(map, bestKind, depth));
            }
        }

        public override double GetDesirability()
        {
            if (!HasFreeSlot())
                return 0.0f;

            IPlayer me = map.GetPlayerMe();
            ISourceAll source = me.GetCollectSourcesNormal();
            int max = 0;
            bestKind = SourceBuildingKind.Count;

            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (source[loop1] > max &&
                    me.GetMonasteryUpgrade((SourceBuildingKind)loop1) != UpgradeKind.SecondUpgrade)
                {
                    max = source[loop1];
                    bestKind = (SourceBuildingKind)loop1;
                }
            }

            double bestSourceDesirability = ((max - 40) / 60.0) / 2.0;
            if (bestSourceDesirability < 0.0)
                return 0.0;
            if (bestSourceDesirability > 0.5)
                bestSourceDesirability = 0.5;

            return bestSourceDesirability;
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
    }
}
