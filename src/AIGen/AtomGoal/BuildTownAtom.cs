using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class BuildTownAtom : AtomGoal
    {
        ITown town;

        public BuildTownAtom(IMapController map, ITown town, int depth) 
            : base(map, depth, "Build town")
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
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
            else
            {
                Log(GoalState.Completed);
                if (map.GetState() == EGameState.StateGame)
                    return GoalState.Completed;
                else
                    return GoalState.Active;
            }
        }
    }
}
