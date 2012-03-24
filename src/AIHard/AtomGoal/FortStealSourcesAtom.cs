using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class FortStealSourcesAtom : AtomGoal
    {
        IPlayer player;

        public FortStealSourcesAtom(IMapController map, IPlayer player, int depth)
            : base(map, depth, "Steal sources")
        {
            this.player = player;
        }

        public override GoalState Process()
        {
            if (map.StealSources(player.GetName()))
            {
                Log(GoalState.Completed);
                return GoalState.Completed;
            }
            else
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
        }
    }
}
