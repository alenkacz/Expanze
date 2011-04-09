using System;
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
            mainGoals.AddLast(new BuildSourceBuilding(map));

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
            if (count > 15)
                count = 6;

            GoalState state = base.Process();

            if (state == GoalState.Failed)
                return GoalState.EndTurn;

            if (state != GoalState.EndTurn)
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
                if (bestGoal != null &&
                    bestFitness > 0.01)
                {
                    subgoals.Enqueue(bestGoal);
                    bestGoal.Init();
                    return Process();
                }

                return GoalState.EndTurn;
            }
            else
                return state;
        }
    }
}
