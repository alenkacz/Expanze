﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildFortAtom : Goal
    {
        ITown town;
        byte pos;

        public BuildFortAtom(IMapController map, ITown town, byte pos)
            : base(map)
        {
            this.town = town;
            this.pos = pos;
        }

        public override GoalState Process()
        {
            if (town.BuildFort(pos) != null)
            {
                map.Log("goal", "BuildFortAtom - sucess");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "BuildFortAtom - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}