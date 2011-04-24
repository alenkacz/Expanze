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
        LinkedList<MainGoal> mainGoals;

        public ThinkGoal(IMapController map, int depth) : base(map, depth, "Think")
        {
            mainGoals = new LinkedList<MainGoal>();
            mainGoals.AddLast(new MainGoal(new BuildTown(map, depth + 1), 0.7));
            mainGoals.AddLast(new MainGoal(new BuildSourceBuilding(map, depth + 1), 0.9));
            mainGoals.AddLast(new MainGoal(new BuildFort(map, depth + 1), 0.2));
            mainGoals.AddLast(new MainGoal(new FortShowParade(map, depth + 1), 0.01));
            mainGoals.AddLast(new MainGoal(new BuildMarket(map, depth + 1), 0.3));
            mainGoals.AddLast(new MainGoal(new BuildMonastery(map, depth + 1), 0.25));
            mainGoals.AddLast(new MainGoal(new InventUpgrade(map, depth + 1), 0.3));
            mainGoals.AddLast(new MainGoal(new BuyLicence(map, depth + 1), 0.5));
            mainGoals.AddLast(new MainGoal(new FortStealSources(map, depth + 1), 1.0));

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
                count = 21;

            GoalState state = base.Process();

            if (state == GoalState.Active)
            {
                if (subgoals.Count > 0 &&
                    subgoals.Peek() is BuildTown &&
                    map.GetState() != EGameState.StateGame)
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
                    bestGoal.Clear();
                    bestGoal.Init();
                    return Process();
                }

                return GoalState.Active;
            }
        }

        public override double GetDesirability()
        {
            throw new NotImplementedException();
        }
    }
}
