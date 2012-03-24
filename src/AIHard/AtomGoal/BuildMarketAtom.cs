using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildMarketAtom : AtomGoal
    {
        ITown town;
        byte pos;

        public BuildMarketAtom(IMapController map, ITown town, byte pos, int depth)
            : base(map, depth, "Build market")
        {
            this.town = town;
            this.pos = pos;
        }

        public override GoalState Process()
        {
            if (town.BuildMarket(pos) != null)
            {
                Log(GoalState.Completed);
                return GoalState.Completed;
            }
            else
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
        }
    }
}
