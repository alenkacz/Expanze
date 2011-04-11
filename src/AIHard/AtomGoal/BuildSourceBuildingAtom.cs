﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildSourceBuildingAtom : Goal
    {
        ITown town;
        byte pos;

        public BuildSourceBuildingAtom(IMapController map, ITown town, byte pos)
            : base(map)
        {
            this.town = town;
            this.pos = pos;
        }

        public override GoalState Process()
        {
            if (town.BuildSourceBuilding(pos))
            {
                map.Log("goal", "SourceBuildingAtom - sucess");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "SourceBuildingAtom - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}