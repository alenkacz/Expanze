using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class FortShowParade : CompositeGoal
    {
        double kHasSources;
        double kPointsToWin;

        public FortShowParade(IMapController map, double kHasSources, int depth)
            : this(map, kHasSources, 1 - kHasSources, depth)
        {
        }

        public FortShowParade(IMapController map, double kHasSources, double kPointsToWin, int depth)
            : base(map, depth, "Show Parade")
        {
            double sum = kHasSources + kPointsToWin;
            this.kHasSources = kHasSources / sum;
            this.kPointsToWin = kPointsToWin / sum;
        }

        public override void Init()
        {
            AddSubgoal(new RaiseSources(map, PriceKind.AParade, depth + 1));
            AddSubgoal(new FortShowParadeAtom(map, depth + 1));
        }

        public override double GetDesirability()
        {
            if(map.IsBanAction(PlayerAction.FortParade))
                return 0.0f;

            if (map.GetPlayerMe().GetBuildingCount(Building.Fort) == 0)
                return 0.0;

            double pointsToWinDesirability = 0.0;// (map.GetPlayerMe().GetPoints() / (double)map.GetGameSettings().GetWinningPoints()) * kPointsToWin;
            double desirability = Desirability.GetHasSources(PriceKind.AParade) * kHasSources + pointsToWinDesirability;

            return desirability;
        }
    }
}
