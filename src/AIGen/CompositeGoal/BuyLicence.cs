using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class BuyLicence : CompositeGoal
    {
        SourceKind bestKind;
        double kPoints;
        double kLicence;

        public BuyLicence(IMapController map, int k, int depth)
            : base(map, depth, "Buy Licence")
        {
            kPoints = k / 100.0f;
            kLicence = 1 - kPoints;

            if (map.GetActionPoints(PlayerPoints.LicenceLvl2) > 0 && map.GetTurnNumber() > 10)
                kPoints += 0.1;
        }

        public override void Init()
        {
            if (bestKind != SourceKind.Count)
            {
                LicenceKind upgrade = LicenceKind.NoLicence;
                switch(map.GetPlayerMe().GetMarketLicence(bestKind))
                {
                    case LicenceKind.NoLicence: upgrade = LicenceKind.FirstLicence; break;
                    case LicenceKind.FirstLicence: upgrade = LicenceKind.SecondLicence; break;
                    case LicenceKind.SecondLicence: return;
                }

                AddSubgoal(new RaiseSources(map, map.GetPrice(bestKind, upgrade), depth + 1));
                AddSubgoal(new BuyLicenceAtom(map, bestKind, depth + 1));
            }
        }

        public override double GetDesirability()
        {
            if (!HasFreeSlot())
                return 0.0f;

            IPlayer me = map.GetPlayerMe();
            ISourceAll source = me.GetCollectSourcesNormal();
            int max = 0;
            bestKind = SourceKind.Null;


            bool hasOnlyFirstLicence = false;
            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (me.GetMarketLicence((SourceKind)loop1) == LicenceKind.FirstLicence)
                {
                    hasOnlyFirstLicence = true;
                    break;
                }
            }

            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (source[loop1] > max &&
                    me.GetMarketLicence((SourceKind)loop1) != LicenceKind.SecondLicence)
                {
                    // nemohu vybrat druhou licenci, kdyz ji nemohu na mape koupit
                    if (me.GetMarketLicence((SourceKind)loop1) == LicenceKind.FirstLicence &&
                        map.CanBuyLicence((SourceKind)loop1) == MarketError.BanSecondLicence)
                        continue;

                    // pokupuji prvni licenci, pokud je vitezstvi za licence levelu 2
                    if (me.GetMarketLicence((SourceKind)loop1) == LicenceKind.NoLicence &&
                        hasOnlyFirstLicence &&
                        map.GetActionPoints(PlayerPoints.LicenceLvl2) > map.GetActionPoints(PlayerPoints.LicenceLvl1))
                    {
                        int debug = 0;
                        debug++;
                        continue;
                    }

                    max = source[loop1];
                    bestKind = (SourceKind)loop1;
                }
            }

            if (bestKind == SourceKind.Null)
                return 0.0f;

            double bestSourceDesirability = ((max - 40) / 60.0) / 2.0;
            if (bestSourceDesirability < 0.0)
                bestSourceDesirability = 0.0;
            if (bestSourceDesirability > 1.0)
                bestSourceDesirability = 1.0;

            double points;
            switch (map.GetPlayerMe().GetMarketLicence(bestKind))
            {
                case LicenceKind.NoLicence:
                    points = map.GetActionPoints(PlayerPoints.LicenceLvl1) + map.GetActionPoints(PlayerPoints.LicenceLvl2);
                    break;
                case LicenceKind.FirstLicence:
                    points = map.GetActionPoints(PlayerPoints.LicenceLvl2);
                    break;
                default:
                    points = 0.0f;
                    break;
            }
            if (bestSourceDesirability < 0.000001 && points < 0.00001)
                return 0.0;

            if (points > 1)
                points = 1.0;

            return kLicence * bestSourceDesirability + kPoints * points;
        }

        private bool HasFreeSlot()
        {
            return map.GetPlayerMe().LicenceFreeSlot > 0;
        }
    }
}
