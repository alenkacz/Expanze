using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum BuildingBuildError { OK, NoSources, AlreadyBuild, NotOwner }
    public enum TownBuildError { OK, NoSources, AlreadyBuild, OtherTownIsClose, NoPlayerRoad }
    public enum BuildingKind { NoBuilding, SourceBuilding, FortBuilding, MarketBuilding, MonasteryBuilding }
    public interface ITownGet
    {
        int getTownID();
        ISourceAll getCost();
        IHexaGet getIHexaGet(int pos);
        TownBuildError CanActivePlayerBuildTown();
    }
}
