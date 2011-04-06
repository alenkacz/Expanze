﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class CompositeGoal : Goal
    {
        protected Stack<Goal> subgoals;

        public CompositeGoal(IMapController map) : base(map)
        {
            subgoals = new Stack<Goal>();
        }

        override public GoalState Process()
        {
            if (subgoals.Count == 0)
                return GoalState.NoSubgoal;

            Goal actualGoal = subgoals.Peek();

            GoalState state = actualGoal.Process();

            switch (state)
            {
                case GoalState.Succesed:
                case GoalState.Failed: 
                    subgoals.Pop(); break;
            }

            return state;
        }

        protected void AddSubgoal(Goal goal)
        {
            subgoals.Push(goal);
        }

        public virtual double GetFitness()
        {
            return 0.0f;
        }
    }
}
