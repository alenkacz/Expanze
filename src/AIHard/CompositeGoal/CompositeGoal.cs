using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
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
                case LogEnd.End: logMsg = "} " + logMsg; break;
            }

            for (int loop1 = 0; loop1 < depth; loop1++)
            {
                logMsg = "  " + logMsg;
            }

            map.Log("goalHiearchy", logMsg);
        }

        override public GoalState Process()
        {
            Log(LogEnd.Start);

            while (subgoals.Count > 0)
            {
                foreach (Goal goal in subgoals)
                {
                    if (!goal.IsStillActual())
                        return GoalState.Failed;
                }

                Goal actualGoal = subgoals.Peek();

                GoalState state = actualGoal.Process();

                switch (state)
                {
                    case GoalState.Failed:
                        subgoals.Clear();
                        return GoalState.Failed;

                    case GoalState.Succesed:
                        subgoals.Dequeue();
                        break;

                    case GoalState.Active:
                        Log(LogEnd.End);
                        return GoalState.Active;
                }
            }

            Log(LogEnd.End);

            return GoalState.Succesed;
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
