using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class BuildSourceBuilding : CompositeGoal
    {
        ITown lastBestTown;
        byte lastBestPos;

        double kBuildingItself;
        double kHasSources;
        double kPoints;

        public BuildSourceBuilding(IMapController map, double kBuildingItself, double kHasSources, double kPoints, int depth)
            : base(map, depth, "Build Source Building")
        {
            double sum = kBuildingItself + kHasSources + kPoints;

            this.kBuildingItself = kBuildingItself / sum;
            this.kHasSources = kHasSources / sum;
            this.kPoints = kPoints / sum;

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

        public double GetDesirability(ITown town, byte pos)
        {
            return GetDesirability(town.GetIHexa(pos));
        }

        public double GetDesirability(IHexa hexa)
        {
            double kindCoef = 1.0;
            int points = 0;
            switch (hexa.GetKind())
            {
                case HexaKind.Desert :
                case HexaKind.Water :
                    return 0.0f;
                case HexaKind.Cornfield :
                    kindCoef = 0.9;
                    points = map.GetActionPoints(PlayerPoints.Mill);
                    break;
                case HexaKind.Forest:
                    kindCoef = 0.8;
                    points = map.GetActionPoints(PlayerPoints.Saw);
                    break;
                case HexaKind.Mountains:
                    kindCoef = 1.2;
                    points = map.GetActionPoints(PlayerPoints.Mine);
                    break;
                case HexaKind.Pasture:
                    kindCoef = 0.9;
                    points = map.GetActionPoints(PlayerPoints.Stepherd);
                    break;
                case HexaKind.Stone:
                    kindCoef = 1.2;
                    points = map.GetActionPoints(PlayerPoints.Quarry);
                    break;
            }

            int startSource = hexa.GetStartSource();

            if (map.GetPlayerMe().GetPoints() + points >= map.GetGameSettings().GetWinningPoints())
                return ThinkGoal.ONE_POINT_REMAIN_FITNESS;

            double desirability = (startSource / 24.0 * kindCoef) * kBuildingItself + Desirability.GetHasSources(hexa.GetSourceBuildingCost()) * kHasSources +
                ((points > 0) ? 1.0 : 0.0) * kPoints;
            return desirability;
        }
    }
}
