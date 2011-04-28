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

        double kBuildingItself;
        double kHasSources;

        public BuildSourceBuilding(IMapController map, double kBuildingItself, double kHasSources, int depth)
            : base(map, depth, "Build Source Building")
        {
            double sum = kBuildingItself + kHasSources;

            this.kBuildingItself = kBuildingItself / sum;
            this.kHasSources = kHasSources / sum;

            lastBestTown = null;
            lastBestPos = 0;
        }

        public override void Init()
        {
            if (lastBestTown == null)
                GetDesirability();

            AddSubgoal(new RaiseSources(map, lastBestTown.GetIHexa(lastBestPos).GetSourceBuildingCost(), depth + 1));
            AddSubgoal(new BuildSourceBuildingAtom(map, lastBestTown, lastBestPos, depth + 1));
            
            lastBestTown = null;
        }

        public override GoalState Process()
        {
            return base.Process();
        }

        public override double GetDesirability()
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

                    tempFitness = GetDesirability(town, loop1);
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

        private double GetDesirability(ITown town, byte pos)
        {
            IHexa hexa = town.GetIHexa(pos);
            if (hexa.GetKind() == HexaKind.Desert ||
               hexa.GetKind() == HexaKind.Water)
                return 0.0f;

            int startSource = hexa.GetStartSource();

            return (startSource / 24.0) * kBuildingItself + Desirability.GetHasSources(hexa.GetSourceBuildingCost()) * kHasSources;
        }
    }
}
