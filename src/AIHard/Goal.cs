using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    public enum GoalState
    {
        Succesed, /// all subgoals were succesful
        Failed,   /// some of subgoals failed
        Active   /// subgoals need next turn
    }

    abstract class Goal
    {
        protected IMapController map;

        public Goal(IMapController map)
        {
            this.map = map;
        }

        public abstract GoalState Process();

        protected bool IsStillActual()
        {
            return true;
        }
    }
}
