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

        public CompositeGoal(IMapController map) : base(map)
        {
            subgoals = new Queue<Goal>();
        }

        public abstract void Init();

        override public GoalState Process()
        {
            if (subgoals.Count == 0)
                return GoalState.Failed;

            Goal actualGoal = subgoals.Peek();

            GoalState state = actualGoal.Process();

            switch (state)
            {
                case GoalState.Succesed:
                case GoalState.Failed: 
                    subgoals.Dequeue(); break;
            }

            return state;
        }

        protected void AddSubgoal(Goal goal)
        {
            subgoals.Enqueue(goal);
        }

        public virtual double GetFitness()
        {
            return 0.0f;
        }
    }
}
