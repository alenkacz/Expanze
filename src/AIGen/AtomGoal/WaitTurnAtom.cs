using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class WaitTurnAtom : AtomGoal
    {
        int turn;

        public WaitTurnAtom(IMapController map, int turn, int depth)
            : base(map, depth, "Wait turn")
        {
            this.turn = turn;
        }

        public override GoalState Process()
        {
            if (turn == 0)
            {
                Log(GoalState.Completed);
                return GoalState.Completed;
            }
            else
            {
                Log(GoalState.Active);
                turn--;
                return GoalState.Active;
            }
        }
    }
}
