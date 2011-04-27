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

        public ThinkGoal(IMapController map, double[] koef, int depth) : base(map, depth, "Think")
        {
            if (koef == null)
            {
                koef = new double[] {0.7, 0.9, 10.0, 0.01, 0.3, 0.25, 0.3, 0.5, 1.0, 7.0 };
            }

            mainGoals = new LinkedList<MainGoal>();
            mainGoals.AddLast(new MainGoal(new BuildTown(map, depth + 1), koef[0]));
            mainGoals.AddLast(new MainGoal(new BuildSourceBuilding(map, depth + 1), koef[1]));
            mainGoals.AddLast(new MainGoal(new BuildFort(map, depth + 1), koef[2]));
            mainGoals.AddLast(new MainGoal(new FortShowParade(map, depth + 1), koef[3]));
            mainGoals.AddLast(new MainGoal(new BuildMarket(map, depth + 1), koef[4]));
            mainGoals.AddLast(new MainGoal(new BuildMonastery(map, depth + 1), koef[5]));
            mainGoals.AddLast(new MainGoal(new InventUpgrade(map, depth + 1), koef[6]));
            mainGoals.AddLast(new MainGoal(new BuyLicence(map, depth + 1), koef[7]));
            mainGoals.AddLast(new MainGoal(new FortStealSources(map, depth + 1), koef[8]));
            mainGoals.AddLast(new MainGoal(new FortCaptureHexa(map, depth + 1), koef[9]));

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
                    Log("");
                    Log("New Plan");
                    Log("  Fitness > " + bestDesirability);
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
