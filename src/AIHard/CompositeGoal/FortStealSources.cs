using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class FortStealSources : CompositeGoal
    {
        IPlayer bestPlayer;

        public FortStealSources(IMapController map)
            : base(map)
        {
        }

        public override void Init()
        {
            if (bestPlayer == null)
            {
                GetDesirability();
            }
            
            AddSubgoal(new RaiseSources(map, PriceKind.AStealSources));
            AddSubgoal(new FortStealSourcesAtom(map, bestPlayer));
            bestPlayer = null;
        }

        public override double GetDesirability()
        {
            double maxDesirability = 0.0;
            double tempDesirability;

            bestPlayer = null;

            foreach (IPlayer p in map.GetPlayerOthers())
            {
                tempDesirability = GetDesirability(p);
                if (tempDesirability > maxDesirability)
                {
                    maxDesirability = tempDesirability;
                    bestPlayer = p;
                }
            }

            return maxDesirability;
        }

        public double GetDesirability(IPlayer player)
        {
            if (map.GetPlayerMe().GetBuildingCount(Building.Fort) == 0)
                return 0.0;

            int sum = 0;
            int sourceHim;
            int sourceMe;
            ISourceAll source = player.GetSource();
            IPlayer me = map.GetPlayerMe();

            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                sourceHim = source.Get((SourceKind)loop1);
                sourceMe = me.GetCollectSourcesNormal().Get((SourceKind)loop1);

                sum += (sourceMe <= 16) ? sourceHim * 2 : sourceHim;
            }

            double result = (sum - 2 * AIHard.SumVector(map.GetPrice(PriceKind.AStealSources).GetAsArray())) / 200.0;
            if (result < 0.0)
                result = 0.0;
            else
                if (result > 1.0)
                    result = 1.0;

            return result;
        }
    }
}
