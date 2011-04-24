﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildRoadAtom : Goal
    {
        IRoad road;

        public BuildRoadAtom(IMapController map, IRoad road, int depth)
            : base(map, depth)
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
                map.Log("goal", "RoadAtom - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
            else
            {
                map.Log("goal", "RoadAtom - succes");
                return GoalState.Succesed;
            }
        }
    }
}
