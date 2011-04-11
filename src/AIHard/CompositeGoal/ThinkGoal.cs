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
        LinkedList<MainGoal> mainGoals;

        public ThinkGoal(IMapController map) : base(map)
        {
            mainGoals = new LinkedList<MainGoal>();
            mainGoals.AddFirst(new MainGoal(new BuildTown(map), 0.7));
            mainGoals.AddLast(new MainGoal(new BuildSourceBuilding(map), 0.9));

            Init();
        }

        int count;
        public override void Init()
        {
            count = 0;
        }

        public override GoalState Process()
        {
            count++;
            if (count > 20)
                count = 6;

            GoalState state = base.Process();

            if (state == GoalState.Active)
            {
                if (subgoals.Count > 0 &&
                    subgoals.Peek() is BuildTown)
                    subgoals.Clear();
                return state;
            }
            else
            {
                CompositeGoal bestGoal = null;
                double bestDesirability = 0.0;
                double tempDesirability;

                double desirabilityCoef;
                CompositeGoal goal;
                foreach (MainGoal mainGoal in mainGoals)
                {
                    goal = mainGoal.goal;
                    desirabilityCoef = mainGoal.desirabilityCoef;

                    tempDesirability = goal.GetDesirability();
                    tempDesirability *= desirabilityCoef;
                    if (tempDesirability > bestDesirability)
                    {
                        bestGoal = goal;
                        bestDesirability = tempDesirability;

                        // fitness more than 1.0 has only goal BuildTown in first two stages of the game
                        if (bestDesirability > 1.1)
                            break;
                    }
                }
                if (bestGoal != null &&
                    bestDesirability > 0.005)
                {
                    subgoals.Enqueue(bestGoal);
                    bestGoal.Init();
                    return Process();
                }

                return GoalState.Active;
            }
        }
    }
}
