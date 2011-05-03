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
            double[] koefMainGoal = koef;
            double[] koefInGoal = null;

            /* from one genetic
            koef = new double[] {
                0.346293775526012,
                0.715893710365469,
                0.0959791071228586,
                0.0884354110287667,
                0.898612148081238,
                0.974229896429102,
                0.756464447247081,
                0.157582921515025,
                0.309927736087669,
                0.781415634221125,
                0.772309043338666,
                0.401954458282308,
                0.473092562273653,
                0.222174956101074,
                0.393841487073266,
                0.911821538075722,
                0.379826089544141,
                0.603927373236011,
                0.781730750008361 
            };
            */

            if (koef != null && koef.Length == 19)
            {
                koefMainGoal = koef.Take<double>(10).ToArray<double>();
                koefInGoal = koef.Skip<double>(10).ToArray<double>();
            }

            if (koefMainGoal == null)
            {
                koefMainGoal = new double[] {0.7, 0.9, 0.5, 0.01, 0.3, 0.25, 0.3, 0.5, 1.0, 0.3 };
            }
            else
            {
                if (koefMainGoal.Length != 10)
                    throw new Exception("Bad main goal length");
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
            else
            {
                if (koefInGoal.Length != 21 && koefInGoal.Length != 9)
                    throw new Exception("Bad koef in goal length");
            }

            mainGoals = new LinkedList<MainGoal>();

            if (koefInGoal.Length == 21)
            {
                mainGoals.AddLast(new MainGoal(new BuildTown(map, koefInGoal[0], koefInGoal[1], depth + 1), koefMainGoal[0]));
                mainGoals.AddLast(new MainGoal(new BuildSourceBuilding(map, koefInGoal[2], koefInGoal[3], depth + 1), koefMainGoal[1]));
                mainGoals.AddLast(new MainGoal(new BuildFort(map, koefInGoal[4], koefInGoal[5], koefInGoal[6], koefInGoal[7], koefInGoal[8], depth + 1), koefMainGoal[2]));
                mainGoals.AddLast(new MainGoal(new FortShowParade(map, koefInGoal[9], koefInGoal[10], depth + 1), koefMainGoal[3]));
                mainGoals.AddLast(new MainGoal(new BuildMarket(map, koefInGoal[11], koefInGoal[12], koefInGoal[13], koefInGoal[14], koefInGoal[15], depth + 1), koefMainGoal[4]));
                mainGoals.AddLast(new MainGoal(new BuildMonastery(map, koefInGoal[16], koefInGoal[17], koefInGoal[18], koefInGoal[19], koefInGoal[20], depth + 1), koefMainGoal[5]));
                mainGoals.AddLast(new MainGoal(new InventUpgrade(map, depth + 1), koefMainGoal[6]));
                mainGoals.AddLast(new MainGoal(new BuyLicence(map, depth + 1), koefMainGoal[7]));
                mainGoals.AddLast(new MainGoal(new FortStealSources(map, depth + 1), koefMainGoal[8]));
                mainGoals.AddLast(new MainGoal(new FortCaptureHexa(map, depth + 1), koefMainGoal[9]));
            }
            else
            {
                mainGoals.AddLast(new MainGoal(new BuildTown(map, koefInGoal[0], depth + 1), koefMainGoal[0]));
                mainGoals.AddLast(new MainGoal(new BuildSourceBuilding(map, koefInGoal[1], depth + 1), koefMainGoal[1]));
                mainGoals.AddLast(new MainGoal(new BuildFort(map, koefInGoal[2], koefInGoal[3], depth + 1), koefMainGoal[2]));
                mainGoals.AddLast(new MainGoal(new FortShowParade(map, koefInGoal[4], depth + 1), koefMainGoal[3]));
                mainGoals.AddLast(new MainGoal(new BuildMarket(map, koefInGoal[5], koefInGoal[6], depth + 1), koefMainGoal[4]));
                mainGoals.AddLast(new MainGoal(new BuildMonastery(map, koefInGoal[7], koefInGoal[8], depth + 1), koefMainGoal[5]));
                mainGoals.AddLast(new MainGoal(new InventUpgrade(map, depth + 1), koefMainGoal[6]));
                mainGoals.AddLast(new MainGoal(new BuyLicence(map, depth + 1), koefMainGoal[7]));
                mainGoals.AddLast(new MainGoal(new FortStealSources(map, depth + 1), koefMainGoal[8]));
                mainGoals.AddLast(new MainGoal(new FortCaptureHexa(map, depth + 1), koefMainGoal[9]));
            }

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
