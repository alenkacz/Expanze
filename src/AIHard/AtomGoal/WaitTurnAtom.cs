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

        public WaitTurnAtom(IMapController map, int turn)
            : base(map)
        {
            this.turn = turn;
        }

        public override GoalState Process()
        {
            if (turn == 0)
            {
                map.Log("goal", "WaitTrun - succes");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "WaitTrun - active > " + turn);
                turn--;
                return GoalState.Active;
            }
        }
    }
}
