using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class BuildRoad : CompositeGoal
    {
        double kPoints;
        double kCantBuildTown;
        IRoad lastBestRoad;


        public BuildRoad(IMapController map, int k, int depth)
            : base(map, depth, "Build road")
        {
            kPoints = k / 1000.0f;
            kCantBuildTown = 1.0f - kPoints;
            lastBestRoad = null;
        }

        public override void Init()
        {

            if (lastBestRoad != null)
            {
                AddSubgoal(new RaiseSources(map, map.GetPrice(PriceKind.BRoad), depth + 1));
                AddSubgoal(new BuildRoadAtom(map, lastBestRoad, depth + 1));
                lastBestRoad = null;
            }
        }

        private bool CanBuildTown()
        {
            ITown tempTown;
            int maxTownID = map.GetMaxTownID();
            for (int loop1 = 1; loop1 < maxTownID; loop1++)
            {
                tempTown = map.GetITownByID(loop1);
                int dst = map.GetDistanceToTown(tempTown, map.GetPlayerMe());
                if (dst < 10)
                    return true;
            }
            return false;
        }

        private int FindBestRoad()
        {
            int maxRoadID = map.GetMaxRoadID();
            List<IPlayer> players = map.GetPlayerOthers();
            IRoad bestRoad = null;
            int best = 99999;

            foreach(IPlayer player in players)
            {
                for (int loop1 = 1; loop1 < maxRoadID; loop1++)
                {
                    RoadBuildError error = map.CanBuildRoad(loop1);
                    if (error == RoadBuildError.NoSources || error == RoadBuildError.OK)
                    {
                        IRoad tempRoad = map.GetIRoadByID(loop1);
                        int temp = map.GetDistanceToRoad(tempRoad, player);
                        if (temp < best)
                        {
                            best = temp;
                            bestRoad = tempRoad;
                        }
                    }
                }
            }
            lastBestRoad = bestRoad;

            return best;
        }

        public override double GetDesirability()
        {
            double points = map.GetActionPoints(PlayerPoints.Road);
            if (points > 1.0) points = 1.0f;
            double cantBuildTown = (CanBuildTown()) ? 0.0f : 1.0f;
            int distToEnemy = FindBestRoad();

            if(lastBestRoad == null)
                return 0.0f;

            double desirabilityWinningBonus = 0.0;
            if (map.GetPlayerMe().GetPoints() + map.GetActionPoints(PlayerPoints.Road) >= map.GetGameSettings().GetWinningPoints())
                desirabilityWinningBonus = ThinkGoal.ONE_POINT_REMAIN_FITNESS;
            return kPoints * points + kCantBuildTown * cantBuildTown + desirabilityWinningBonus;
        }
    }
}
