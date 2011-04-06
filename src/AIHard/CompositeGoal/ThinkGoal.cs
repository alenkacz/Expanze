﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIHard;
using CorePlugin;

namespace AIHard
{
    class ThinkGoal : CompositeGoal
    {
        LinkedList<CompositeGoal> mainGoals;

        public ThinkGoal(IMapController map) : base(map)
        {
            mainGoals = new LinkedList<CompositeGoal>();
            mainGoals.AddFirst(new BuildTown(map));
        }

        public override GoalState Process()
        {
            GoalState state = base.Process();

            if (state == GoalState.NoSubgoal)
            {
                CompositeGoal bestGoal = null;
                double bestFitness = 0.0;
                double tempFitness;
                foreach (CompositeGoal goal in mainGoals)
                {
                    tempFitness = goal.GetFitness();
                    if (tempFitness > bestFitness)
                    {
                        bestGoal = goal;
                        bestFitness = tempFitness;

                        // fitness more than 1.0 has only goal BuildTown in first two stages of the game
                        if (bestFitness > 1.1)
                            break;
                    }
                }
                if (bestGoal != null)
                {
                    subgoals.Push(bestGoal);
                    return Process();
                }

                return GoalState.NoSubgoal;
            }
            else
                return state;
        }
    }
}