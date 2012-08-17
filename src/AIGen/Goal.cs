using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    public enum GoalState
    {
        Completed, /// all subgoals were succesful
        Failed,   /// some of subgoals failed
        Active   /// subgoals need next turn
    }

    public enum LogEnd
    {
        Start,
        End,
        Middle
    }

    abstract class Goal
    {
        protected int depth;
        protected string name;
        protected IMapController map;

        public Goal(IMapController map, int depth, string name)
        {
            this.map = map;
            this.depth = depth;
            this.name = name;
        }

        public abstract GoalState Process();

        protected void Log(string msg)
        {
            map.Log("goalHiearchy" + map.GetPlayerMe().GetName(), msg);
        }

        public virtual bool IsStillActual()
        {
            return true;
        }
    }
}
