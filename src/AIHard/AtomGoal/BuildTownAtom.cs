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
            {
                map.Log("goal.txt", "TownAtom - failed");
                return GoalState.Failed;
            }
            else
            {
                if (map.GetState() == EGameState.StateGame)
                {
                    map.Log("goal.txt", "TownAtom - succes");
                    return GoalState.Succesed;
                }
                else
                {
                    map.Log("goal.txt", "TownAtom - endturn");
                    return GoalState.EndTurn;
                }
            }
        }
    }
}
