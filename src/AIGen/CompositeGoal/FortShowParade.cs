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

        public FortShowParade(IMapController map, int k, int depth)
            : base(map, depth, "Show Parade")
        {
            this.kHasSources = k / 1000.0f;
            this.kPoints = 1- kHasSources;
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
