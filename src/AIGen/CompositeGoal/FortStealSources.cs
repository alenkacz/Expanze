using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class FortStealSources : CompositeGoal
    {
        IPlayer bestPlayer;

        double kPoints;
        double kSteal;

        public FortStealSources(IMapController map, double kPoints, int depth)
            : base(map, depth, "Steal sources")
        {
            this.kPoints = kPoints;
            kSteal = 1 - kPoints;
        }

        public override void Init()
        {
            if (bestPlayer == null)
            {
                GetDesirability();
            }
            
            AddSubgoal(new RaiseSources(map, PriceKind.AStealSources, depth + 1));
            AddSubgoal(new FortStealSourcesAtom(map, bestPlayer, depth + 1));
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

            double points = (map.GetActionPoints(PlayerPoints.FortStealSources) > 0) ? 1.0 : 0.0;
            return maxDesirability * kSteal + points * kPoints;
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

            double result = (sum - 2 * AIGen.SumVector(map.GetPrice(PriceKind.AStealSources).GetAsArray())) / 200.0;
            if (result < 0.0)
                result = 0.0;
            else
                if (result > 1.0)
                    result = 1.0;

            return result;
        }
    }
}
