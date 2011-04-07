using System;
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
                return GoalState.Failed;
            else
                return GoalState.Succesed;
        }
    }
}
