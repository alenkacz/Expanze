using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildFortAtom : AtomGoal
    {
        ITown town;
        byte pos;

        public BuildFortAtom(IMapController map, ITown town, byte pos, int depth)
            : base(map, depth, "Build fort")
        {
            this.town = town;
            this.pos = pos;
        }

        public override GoalState Process()
        {
            if (town.BuildFort(pos) != null)
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
