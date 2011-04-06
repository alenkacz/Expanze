using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildTownAtom : Goal
    {
        ITown town;

        public BuildTownAtom(IMapController map, ITown town) : base(map)
        {
            this.town = town;
        }

        public override GoalState Process()
        {
            if (town.Build() == null)
                return GoalState.Failed;
            else
                return GoalState.Succesed;
        }
    }
}
