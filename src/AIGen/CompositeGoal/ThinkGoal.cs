using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIGen;
using CorePlugin;

namespace AIGen
{
    class ThinkGoal : CompositeGoal
    {
        LinkedList<MainGoal> mainGoals;

        public ThinkGoal(IMapController map, double[] koef, int depth) : base(map, depth, "Think")
        {
            double[] koefMainGoal = koef;
            double[] koefInGoal = null;

            if (koef != null && koef.Length == 19)
            {
                koefMainGoal = koef.Take<double>(10).ToArray<double>();
                koefInGoal = koef.Skip<double>(10).ToArray<double>();
            }


            if (koefInGoal == null)
            {
                koefInGoal = new double[] {0.2, 0.8,                 // Build town
                                           0.5, 0.5,                 // Build source building
                                           0.6, 0.2, 0.2, 0.2, 0.05, // Build Fort
                                           0.25, 0.75,               // Show parade
                                           0.375, 0.125, 0.250, 0.250, 0.01, // Build Market
                                           0.375, 0.125, 0.250, 0.250, 0.01  // Build Monastery
                };
            }

            mainGoals = new LinkedList<MainGoal>();

            
            mainGoals.AddLast(new MainGoal(new BuildTown(map, 0.5, 0.2, 0.3, depth + 1), koefMainGoal[0]));
            mainGoals.AddLast(new MainGoal(new BuildRoad(map, 0.5, depth + 1), 0.1f));
            mainGoals.AddLast(new MainGoal(new BuildSourceBuilding(map, 0.3, 0.4, 0.3, depth + 1), koefMainGoal[1]));
            if (!map.IsBanAction(PlayerAction.BuildFort))
                mainGoals.AddLast(new MainGoal(new BuildFort(map, 0.1, 0.2, 0.3, 0.4, 0.5, depth + 1), koefMainGoal[2]));
            if (!map.IsBanAction(PlayerAction.FortParade))
                mainGoals.AddLast(new MainGoal(new FortShowParade(map, 0.4, depth + 1), koefMainGoal[3]));
            if (!map.IsBanAction(PlayerAction.BuildMarket))
                mainGoals.AddLast(new MainGoal(new BuildMarket(map, 0.1, 0.2, 0.3, 0.1, 0.1, 0.2, depth + 1), koefMainGoal[4]));
            if (!map.IsBanAction(PlayerAction.BuildMonastery))
                mainGoals.AddLast(new MainGoal(new BuildMonastery(map, 0.1, 0.2, 0.1, 0.3, 0.1, 0.2, depth + 1), koefMainGoal[5]));
            mainGoals.AddLast(new MainGoal(new InventUpgrade(map, 0.4, depth + 1), koefMainGoal[6]));
            mainGoals.AddLast(new MainGoal(new BuyLicence(map, 0.6, depth + 1), koefMainGoal[7]));
            if (!map.IsBanAction(PlayerAction.FortStealSources))
                mainGoals.AddLast(new MainGoal(new FortStealSources(map, 0.5, depth + 1), koefMainGoal[8]));
            if (!map.IsBanAction(PlayerAction.FortCaptureHexa))
                mainGoals.AddLast(new MainGoal(new FortCaptureHexa(map, 0.5, depth + 1), koefMainGoal[9]));

            Init();
        }

        int count;
        public override void Init()
        {
            count = 0;
        }

        public override GoalState Process()
        {
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
                    if (count > 40)
                    {
                        count = 1000;
                        return GoalState.Failed;
                    }
                    Log("");
                    Log("New Plan");
                    Log("  Fitness > " + bestDesirability);
                    subgoals.Enqueue(bestGoal);
                    bestGoal.Clear();
                    bestGoal.Init();
                    count++;
                    GoalState gstate = Process();
                    count--;
                    return gstate;
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
