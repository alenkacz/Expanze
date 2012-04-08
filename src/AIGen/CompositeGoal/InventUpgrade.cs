using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class InventUpgrade : CompositeGoal
    {
        SourceBuildingKind bestKind;

        double kPoints;
        double kUpgrade;

        public InventUpgrade(IMapController map, double kPoints, int depth)
            : base(map, depth, "Invent upgrade")
        {
            this.kPoints = kPoints;
            kUpgrade = 1 - kPoints;
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

                AddSubgoal(new RaiseSources(map, map.GetPrice(bestKind, upgrade), depth + 1));
                AddSubgoal(new InventUpgradeAtom(map, bestKind, depth + 1));
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
                    if (me.GetMonasteryUpgrade((SourceBuildingKind)loop1) == UpgradeKind.FirstUpgrade &&
                        map.CanInventUpgrade((SourceBuildingKind)loop1) == MonasteryError.BanSecondUpgrade)
                        continue;

                    max = source[loop1];
                    bestKind = (SourceBuildingKind)loop1;
                }
            }

            double bestSourceDesirability = ((max - 40) / 60.0) / 2.0;
            if (bestSourceDesirability < 0.0)
                return 0.0;
            if (bestSourceDesirability > 1.0)
                bestSourceDesirability = 1.0;

            double points;
            switch (map.GetPlayerMe().GetMonasteryUpgrade(bestKind))
            {
                case UpgradeKind.NoUpgrade :
                    points = map.GetActionPoints(PlayerPoints.UpgradeLvl1) + map.GetActionPoints(PlayerPoints.UpgradeLvl2);
                    break;
                case UpgradeKind.FirstUpgrade:
                    points = map.GetActionPoints(PlayerPoints.UpgradeLvl2);
                    break;
                default :
                    points = 0.0f;
                    break;
            }
            if (points > 1)
                points = 1.0;

            return kUpgrade * bestSourceDesirability + kPoints * points;
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
