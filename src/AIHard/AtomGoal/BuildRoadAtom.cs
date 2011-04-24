using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildRoadAtom : AtomGoal
    {
        IRoad road;

        public BuildRoadAtom(IMapController map, IRoad road, int depth)
            : base(map, depth, "Build road")
        {
            this.road = road;
        }

        public override bool IsStillActual()
        {
            return !road.GetIsBuild();
        }

        public override GoalState Process()
        {
            if (road.Build() == null)
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
            else
            {
                Log(GoalState.Succesed);
                return GoalState.Succesed;
            }
        }
    }
}
