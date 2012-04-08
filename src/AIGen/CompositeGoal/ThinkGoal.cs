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

        public ThinkGoal(IMapController map, int[][] koef, int depth) : base(map, depth, "Think")
        {
            if (koef == null)
            {
                koef = new int[11][];
                koef[0] = new int[4];
                koef[1] = new int[2];
                koef[2] = new int[4];
                koef[3] = new int[5];
                koef[4] = new int[2];
                koef[5] = new int[6];
                koef[6] = new int[6];
                koef[7] = new int[2];
                koef[8] = new int[2];
                koef[9] = new int[2];
                koef[10] = new int[2];

                koef[0] = new int[] { 1, 300, 500, 100 };
                koef[1] = new int[] {4, 100};
                koef[2] = new int[] {1, 200, 100, 200};
                koef[3] = new int[] {1, 2, 3, 4, 5};
                koef[4] = new int[] {1, 200};
                koef[5] = new int[] {1, 2, 3, 4, 5, 6};
                koef[6] = new int[] {1, 2, 3, 4, 5, 6};
                koef[7] = new int[] {1, 100};
                koef[8] = new int[] {1, 200};
                koef[9] = new int[] {2, 300};
                koef[10] = new int[] {3, 100};
            }

            mainGoals = new LinkedList<MainGoal>();
            mainGoals.AddLast(new MainGoal(new BuildTown(map, koef[0][1], koef[0][2], koef[0][3], depth + 1), koef[0][0]));
            mainGoals.AddLast(new MainGoal(new BuildRoad(map, koef[1][1], depth + 1), koef[1][0]));
            mainGoals.AddLast(new MainGoal(new BuildSourceBuilding(map, koef[2][1], koef[2][2], koef[2][3], depth + 1), koef[2][0]));

            if (!map.IsBanAction(PlayerAction.BuildFort))
                mainGoals.AddLast(new MainGoal(new BuildFort(map, koef[3][1], koef[3][2], koef[3][3], koef[3][4], depth + 1), koef[3][0]));
            if (!map.IsBanAction(PlayerAction.FortParade))
                mainGoals.AddLast(new MainGoal(new FortShowParade(map, koef[4][1], depth + 1), koef[4][0]));
            if (!map.IsBanAction(PlayerAction.BuildMarket))
                mainGoals.AddLast(new MainGoal(new BuildMarket(map, koef[5][1], koef[5][2], koef[5][3], koef[5][4], koef[5][5], depth + 1), koef[5][0]));
            if (!map.IsBanAction(PlayerAction.BuildMonastery))
                mainGoals.AddLast(new MainGoal(new BuildMonastery(map, koef[6][1], koef[6][2], koef[6][3], koef[6][4], koef[6][5], depth + 1), koef[6][0]));
            mainGoals.AddLast(new MainGoal(new InventUpgrade(map, koef[7][1], depth + 1), koef[7][0]));
            mainGoals.AddLast(new MainGoal(new BuyLicence(map, koef[8][1], depth + 1), koef[8][0]));
            if (!map.IsBanAction(PlayerAction.FortStealSources))
                mainGoals.AddLast(new MainGoal(new FortStealSources(map, koef[9][1], depth + 1), koef[9][0]));
            if (!map.IsBanAction(PlayerAction.FortCaptureHexa))
                mainGoals.AddLast(new MainGoal(new FortCaptureHexa(map, koef[10][1], depth + 1), koef[10][0]));

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
                    tempDesirability *= desirabilityCoef / 1000.0f;
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
