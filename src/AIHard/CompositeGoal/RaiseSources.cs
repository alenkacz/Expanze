using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class RaiseSources : CompositeGoal
    {
        List<ISourceAll> source;

        public RaiseSources(IMapController map, List<ISourceAll> sourceList)
            : base(map)
        {
            this.source = sourceList;
            Init();
        }

        public RaiseSources(IMapController map, PriceKind priceKind)
            : this(map, map.GetPrice(priceKind))
        {
        }

        public RaiseSources(IMapController map, ISourceAll source)
            : base(map)
        {
            List<ISourceAll> sourceList = new List<ISourceAll>();
            sourceList.Add(source);
            this.source = sourceList;
            Init();
        }

        private int[] SumSourceList(List<ISourceAll> list)
        {
            int[] sum = new int[5];

            foreach (ISourceAll s in list)
            {
                sum = AIHard.Sum2Vectors(sum, s.GetAsArray());
            }

            return sum;
        }

        private int GetNTurnsToWait()
        {
            int[] sourceCost = SumSourceList(source);
            int[] sourceNow = map.GetPlayerMe().GetSource().GetAsArray();
            int[] sourcePerTurn = map.GetPlayerMe().GetCollectSourcesNormal().GetAsArray();

            bool[] dontHaveToChange = new bool[5];
            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                dontHaveToChange[loop1] = false;

                if (sourceNow[loop1] > sourceCost[loop1] || sourcePerTurn[loop1] > 0)
                    dontHaveToChange[loop1] = true;

            }

            int turn = 0;
            bool done = false;
            do
            {
                done = true;
                for (int loop1 = 0; loop1 < 5; loop1++)
                {
                    if (sourceNow[loop1] < sourceCost[loop1] &&
                       dontHaveToChange[loop1])
                    {
                        done = false;
                        turn++;
                        sourceNow = AIHard.Sum2Vectors(sourceNow, sourcePerTurn);
                    }
                }

            }  while (!done);

            return turn;
        }

        public override void Init()
        {
            {
                int turn = GetNTurnsToWait();

                if (Desirability.HasSomeoneBuilding(Building.Fort))
                {
                    if (AIHard.SumVector(map.GetPlayerMe().GetSource().GetAsArray()) * 3.0 / 2.0 > AIHard.SumVector(map.GetPrice(PriceKind.AStealSources).GetAsArray()))
                    {
                        turn = 0;
                    }
                    else if(turn > 1)
                        turn = 1;
                }

                AddSubgoal(new WaitTurnAtom(map, (turn <= 2) ? turn : 2));
                AddSubgoal(new ChangeSourcesAtom(map, source));
            }
        }

        public override GoalState Process()
        {
            GoalState state = base.Process();

            if (state == GoalState.Failed)
            {
                AddSubgoal(new ChangeSourcesAtom(map, source));
                return GoalState.Active;
            }

            return state;
        }

        public override double GetDesirability()
        {
            return 0.0f;
        }
    }
}
