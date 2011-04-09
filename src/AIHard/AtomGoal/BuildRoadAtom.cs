using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildRoadAtom : Goal
    {
        IRoad road;

        public BuildRoadAtom(IMapController map, IRoad road)
            : base(map)
        {
            this.road = road;
        }

        public override GoalState Process()
        {
            if (road.Build() == null)
            {
                map.Log("goal.txt", "RoadAtom - sucess");
                return GoalState.Failed;
            }
            else
            {
                map.Log("goal.txt", "RoadAtom - failed");
                return GoalState.Succesed;
            }
        }
    }
}
