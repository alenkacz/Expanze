using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildMonasteryAtom : Goal
    {
        ITown town;
        byte pos;

        public BuildMonasteryAtom(IMapController map, ITown town, byte pos, int depth)
            : base(map, depth)
        {
            this.town = town;
            this.pos = pos;
        }

        public override GoalState Process()
        {
            if (town.BuildMonastery(pos) != null)
            {
                map.Log("goal", "BuildMonasteryAtom - sucess");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "BuildMonasteryAtom - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}
