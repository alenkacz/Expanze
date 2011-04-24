using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildMonasteryAtom : AtomGoal
    {
        ITown town;
        byte pos;

        public BuildMonasteryAtom(IMapController map, ITown town, byte pos, int depth)
            : base(map, depth, "Build monastery")
        {
            this.town = town;
            this.pos = pos;
        }

        public override GoalState Process()
        {
            if (town.BuildMonastery(pos) != null)
            {
                Log(GoalState.Succesed);
                return GoalState.Succesed;
            }
            else
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
        }
    }
}
