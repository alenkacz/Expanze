using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    /// <summary>
    /// OK No error, player can build road
    /// NoSources Player has not enough sources
    /// AlreadyBuild Someone has already build road on selected place
    /// NoPlayerRoadOrTown Selected place is not connect with players road or town
    /// </summary>
    public enum RoadBuildError { OK, NoSources, AlreadyBuild, NoPlayerRoadOrTown, InvalidRoadID }

    public interface IRoad
    {
        RoadBuildError CanBuildRoad();
        IRoad Build();

        int GetRoadID();
    }
}
