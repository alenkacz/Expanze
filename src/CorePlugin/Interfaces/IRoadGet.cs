using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum RoadBuildError { OK, NoSources, AlreadyBuild, NoPlayerRoadOrTown }

    public interface IRoadGet
    {
        RoadBuildError CanActivePlayerBuildRoad();
    }
}
