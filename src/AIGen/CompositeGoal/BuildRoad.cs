using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class BuildRoad : CompositeGoal
    {
        double kHasSources;
        double kPointsToWin;

        public BuildRoad(IMapController map, double kHasSources, int depth)
            : this(map, depth)
        {
        }

        public BuildRoad(IMapController map, int depth)
            : base(map, depth, "Build road")
        {
        }

        public override void Init()
        {
            AddSubgoal(new RaiseSources(map, map.GetPrice(PriceKind.BRoad), depth + 1));
            //AddSubgoal(new BuildRoadAtom(map, path[loop1], depth + 1));
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

        public override double GetDesirability()
        {
            double points = map.GetActionPoints(PlayerPoints.Road) / 3.0f;
            if (points > 1.0) points = 1.0f;
            double cantBuildTown = (CanBuildTown()) ? 0.0f : 1.0f;

            

            return (points + cantBuildTown) / 2.0f;
        }
    }
}
