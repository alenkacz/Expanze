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
        protected int depth;

        protected IMapController map;

        public Goal(IMapController map, int depth)
        {
            this.map = map;
            this.depth = depth;
        }

        public abstract GoalState Process();

        public virtual bool IsStillActual()
        {
            return true;
        }
    }
}
