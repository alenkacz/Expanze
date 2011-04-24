using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class WaitTurnAtom : Goal
    {
        int turn;

        public WaitTurnAtom(IMapController map, int turn, int depth)
            : base(map, depth, "Wait turn Atom")
        {
            this.turn = turn;
        }

        public override GoalState Process()
        {
            if (turn == 0)
            {
                map.Log("goal", "WaitTurn - succes");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "WaitTurn - active > " + turn);
                turn--;
                return GoalState.Active;
            }
        }
    }
}
