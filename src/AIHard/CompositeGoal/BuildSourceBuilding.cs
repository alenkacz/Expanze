using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuildSourceBuilding : CompositeGoal
    {
        ITown lastBestTown;
        byte lastBestPos;

        public BuildSourceBuilding(IMapController map)
            : base(map)
        {
            lastBestTown = null;
            lastBestPos = 0;
        }

        public override void Init()
        {
            if (lastBestTown == null)
                GetFitness();
            
            subgoals.Enqueue(new BuildSourceBuildingAtom(map, lastBestTown, lastBestPos));
            
            lastBestTown = null;
        }

        public override GoalState Process()
        {
            return base.Process();
        }

        public override double GetFitness()
        {
            List<ITown> towns = map.GetPlayerMe().GetTown();

            double bestFitness = 0.0;
            double tempFitness;
            lastBestTown = null;

            foreach (ITown town in towns)
            {
                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    if (town.GetBuildingKind(loop1) != BuildingKind.NoBuilding)
                        continue;

                    tempFitness = GetFitness(town, loop1);
                    if (tempFitness > bestFitness)
                    {
                        lastBestTown = town;
                        lastBestPos = loop1;
                        bestFitness = tempFitness;
                    }
                }
            }

            return bestFitness;
        }

        private double GetFitness(ITown town, byte pos)
        {
            int startSource = town.GetIHexa(pos).GetStartSource();

            return startSource / 24.0;
        }
    }
}
