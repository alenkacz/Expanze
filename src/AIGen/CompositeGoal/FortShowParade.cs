using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class FortShowParade : CompositeGoal
    {
        double kHasSources;
        double kPoints;

        public FortShowParade(IMapController map, double kHasSources, int depth)
            : this(map, kHasSources, 1 - kHasSources, depth)
        {
        }

        public FortShowParade(IMapController map, double kHasSources, double kPoints, int depth)
            : base(map, depth, "Show Parade")
        {
            double sum = kHasSources + kPoints;
            this.kHasSources = kHasSources / sum;
            this.kPoints = kPoints / sum;
        }

        public override void Init()
        {
            AddSubgoal(new RaiseSources(map, PriceKind.AParade, depth + 1));
            AddSubgoal(new FortShowParadeAtom(map, depth + 1));
        }

        public override double GetDesirability()
        {
            if (map.GetPlayerMe().GetBuildingCount(Building.Fort) == 0)
                return 0.0;

            double pointsDesirability = (map.GetActionPoints(PlayerPoints.FortParade) > 0) ? 1.0 : 0.0;
            double desirability = Desirability.GetHasSources(PriceKind.AParade) * kHasSources + pointsDesirability * kPoints;

            return desirability;
        }
    }
}
