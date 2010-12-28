using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum BuildingBuildError { OK, NoSources, AlreadyBuild, NotOwner, TownHasNoHexaWithThatHexaID, NoSourceBuildingForDesert, InvalidTownID }
    public enum TownBuildError { OK, NoSources, AlreadyBuild, OtherTownIsClose, NoPlayerRoad, InvalidTownID }
    public enum BuildingKind { NoBuilding, SourceBuilding, FortBuilding, MarketBuilding, MonasteryBuilding }
    
    public interface ITown
    {
        TownBuildError CanActivePlayerBuildTown();
        ITown Build();
        ISourceBuilding BuildSourceBuilding(int pos);

        int GetTownID();
        ISourceAll GetCost();
        IHexa GetIHexaGet(int pos);
    }
}
