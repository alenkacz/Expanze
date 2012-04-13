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
        int sumSources;
        int level;              // which array of koef should player use
        int[][] coeficients;

        public ThinkGoal(IMapController map, int[][] koef, int depth) : base(map, depth, "Think")
        {
            sumSources = 0;
            level = 0;
            coeficients = koef;

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

                coeficients = koef;
            }

            AddMainGoals();
        }

        private void AddMainGoals()
        {
            int offset = 0;
            if (coeficients.Length == 11)
                offset = 0;
            else if(coeficients.Length == 22 && level == 1)
                offset = 11;

            mainGoals = new LinkedList<MainGoal>();
            mainGoals.AddLast(new MainGoal(new BuildTown(map, coeficients[offset + 0][1], coeficients[offset + 0][2], coeficients[offset + 0][3], depth + 1), coeficients[offset + 0][0]));
            mainGoals.AddLast(new MainGoal(new BuildRoad(map, coeficients[offset + 1][1], depth + 1), coeficients[offset + 1][0]));
            mainGoals.AddLast(new MainGoal(new BuildSourceBuilding(map, coeficients[offset + 2][1], coeficients[offset + 2][2], coeficients[offset + 2][3], depth + 1), coeficients[offset + 2][0]));

            if (!map.IsBanAction(PlayerAction.BuildFort))
            {
                mainGoals.AddLast(new MainGoal(new BuildFort(map, coeficients[offset + 3][1], coeficients[offset + 3][2], coeficients[offset + 3][3], coeficients[offset + 3][4], depth + 1), coeficients[offset + 3][0]));
                if (!map.IsBanAction(PlayerAction.FortParade))
                    mainGoals.AddLast(new MainGoal(new FortShowParade(map, coeficients[offset + 4][1], depth + 1), coeficients[offset + 4][0]));
                if (!map.IsBanAction(PlayerAction.FortStealSources))
                    mainGoals.AddLast(new MainGoal(new FortStealSources(map, coeficients[offset + 9][1], depth + 1), coeficients[offset + 9][0]));
                if (!map.IsBanAction(PlayerAction.FortCaptureHexa))
                    mainGoals.AddLast(new MainGoal(new FortCaptureHexa(map, coeficients[offset + 10][1], depth + 1), coeficients[offset + 10][0]));
            }
            if (!map.IsBanAction(PlayerAction.BuildMarket))
            {
                mainGoals.AddLast(new MainGoal(new BuildMarket(map, coeficients[offset + 5][1], coeficients[offset + 5][2], coeficients[offset + 5][3], coeficients[offset + 5][4], coeficients[offset + 5][5], depth + 1), coeficients[offset + 5][0]));
                mainGoals.AddLast(new MainGoal(new BuyLicence(map, coeficients[offset + 8][1], depth + 1), coeficients[offset + 8][0]));
            }
            if (!map.IsBanAction(PlayerAction.BuildMonastery))
            {
                mainGoals.AddLast(new MainGoal(new BuildMonastery(map, coeficients[offset + 6][1], coeficients[offset + 6][2], coeficients[offset + 6][3], coeficients[offset + 6][4], coeficients[offset + 6][5], depth + 1), coeficients[offset + 6][0]));
                mainGoals.AddLast(new MainGoal(new InventUpgrade(map, coeficients[offset + 7][1], depth + 1), coeficients[offset + 7][0]));
            }
        }

        int count;
        public override void Init()
        {
            count = 0;

            if (level == 0)
            {
                if (sumSources == 0)
                {
                    int hexaNumber = map.GetMaxHexaID();
                    for (int loop1 = 1; loop1 <= hexaNumber; loop1++)
                    {
                        sumSources += map.GetIHexaByID(loop1).GetStartSource();
                    }

                    sumSources *= 3;
                }

                int meSources = map.GetPlayerMe().GetCollectSourcesNormal().GetAsArray().Sum();

                if (meSources > sumSources / 2 / (map.GetPlayerOthers().Count + 1) ||
                    meSources > 150)
                {
                    UpgradeMe();
                }
            }
        }

        private void UpgradeMe()
        {
            level++;

            mainGoals.Clear();
            AddMainGoals();
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
                        if (bestDesirability > 100000.0)
                            break;
                    }
                }
                if (bestGoal != null &&
                    bestDesirability > 0.00001)
                {
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
