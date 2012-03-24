using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildSourceBuildingAtom : AtomGoal
    {
        ITown town;
        byte pos;

        public BuildSourceBuildingAtom(IMapController map, ITown town, byte pos, int depth)
            : base(map, depth, "Build source building")
        {
            this.town = town;
            this.pos = pos;
        }

        public override GoalState Process()
        {
            if (town.BuildSourceBuilding(pos))
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
