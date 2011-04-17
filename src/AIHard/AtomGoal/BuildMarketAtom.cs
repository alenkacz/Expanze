﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildMarketAtom : Goal
    {
        ITown town;
        byte pos;

        public BuildMarketAtom(IMapController map, ITown town, byte pos)
            : base(map)
        {
            this.town = town;
            this.pos = pos;
        }

        public override GoalState Process()
        {
            if (town.BuildMarket(pos) != null)
            {
                map.Log("goal", "BuildMarketAtom - sucess");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "BuildMarketAtom - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}
