using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    public enum GoalState
    {
        Succesed,
        Failed,
        EndTurn
    }

    abstract class Goal
    {
        protected IMapController map;

        public Goal(IMapController map)
        {
            this.map = map;
        }

        public abstract GoalState Process();
    }
}
