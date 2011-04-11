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

        public override bool IsStillActual()
        {
            return town.IsPossibleToBuildTown();
        }

        public override GoalState Process()
        {
            if (town.Build() == null)
            {
                map.Log("goal", "TownAtom - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
            else
            {
                map.Log("goal", "TownAtom - succes");
                if (map.GetState() == EGameState.StateGame)
                    return GoalState.Succesed;
                else
                    return GoalState.Active;
            }
        }
    }
}
