using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class FortShowParade : CompositeGoal
    {
        public FortShowParade(IMapController map)
            : base(map)
        {
        }

        public override void Init()
        {
            AddSubgoal(new RaiseSources(map, PriceKind.AParade));
            AddSubgoal(new FortShowParadeAtom(map));
        }

        public override double GetDesirability()
        {
            if (map.GetPlayerMe().GetBuildingCount(Building.Fort) == 0)
                return 0.0;

            double pointsToWinDesirability = (map.GetPlayerMe().GetPoints() / (double) map.GetGameSettings().GetWinningPoints()) / 3.0 * 2.0;
            double desirability = Desirability.GetHasSources(PriceKind.AParade) / 3.0 + pointsToWinDesirability;

            return desirability;
        }
    }
}
