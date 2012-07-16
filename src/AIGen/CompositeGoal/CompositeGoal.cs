using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    abstract class CompositeGoal : Goal
    {
        protected Queue<Goal> subgoals;

        public CompositeGoal(IMapController map, int depth, string name) : base(map, depth, name)
        {
            subgoals = new Queue<Goal>();
        }

        public abstract void Init();

        protected void Log(LogEnd end)
        {
            string logMsg = "";

            logMsg += name;

            switch (end)
            {
                case LogEnd.Start: logMsg += " {"; break;
                case LogEnd.End: logMsg = "} "; break;
                case LogEnd.Middle: logMsg = "} FAIL"; break;
            }

            for (int loop1 = 0; loop1 < depth; loop1++)
            {
                logMsg = "  " + logMsg;
            }

            Log(logMsg);
        }

        override public GoalState Process()
        {
            bool wasEmpty = false;
            if (subgoals.Count > 0)
            {
                Log(LogEnd.Start);
            }
            else
                wasEmpty = true;

            while (subgoals.Count > 0)
            {
                foreach (Goal goal in subgoals)
                {
                    if (!goal.IsStillActual())
                    {
                        Log(LogEnd.Middle);
                        return GoalState.Failed;
                    }
                }

                Goal actualGoal = subgoals.Peek();

                GoalState state = actualGoal.Process();

                switch (state)
                {
                    case GoalState.Failed:
                        subgoals.Clear();
                        return GoalState.Failed;

                    case GoalState.Completed:
                        subgoals.Dequeue();
                        break;

                    case GoalState.Active:
                        Log(LogEnd.End);
                        return GoalState.Active;
                }
            }

            if(!wasEmpty)
                Log(LogEnd.End);

            return GoalState.Completed;
        }

        protected void AddSubgoal(Goal goal)
        {
            subgoals.Enqueue(goal);
        }

        public abstract double GetDesirability();

        internal void Clear()
        {
            subgoals.Clear();
        }
    }
}
