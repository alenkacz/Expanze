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

            double des = Desirability.GetHasSources(PriceKind.AParade);

            return des;
        }
    }
}
