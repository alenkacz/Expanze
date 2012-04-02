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

        public BuyLicence(IMapController map, int depth)
            : base(map, depth, "Buy Licence")
        {
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
            bestKind = SourceKind.Count;

            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (source[loop1] > max &&
                    me.GetMarketLicence((SourceKind)loop1) != LicenceKind.SecondLicence)
                {
                    max = source[loop1];
                    bestKind = (SourceKind)loop1;
                }
            }

            double bestSourceDesirability = ((max - 40) / 60.0) / 2.0;
            if (bestSourceDesirability < 0.0)
                return 0.0;
            if (bestSourceDesirability > 1.0)
                bestSourceDesirability = 1.0;

            return bestSourceDesirability;
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
    }
}
