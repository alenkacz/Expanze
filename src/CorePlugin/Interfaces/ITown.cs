using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum BuildingBuildError { OK, NoSources, AlreadyBuild, NotOwner, TownHasNoHexaWithThatHexaID, NoSourceBuildingForDesert, InvalidTownID, NoBuildingForWater, NoSpecialBuildingForMountains, Ban }
    public enum TownBuildError { OK, NoSources, AlreadyBuild, OtherTownIsClose, NoPlayerRoad, InvalidTownID, YouHaveBuiltTownThisTurn }
    public enum BuildingKind { NoBuilding, SourceBuilding, FortBuilding, MarketBuilding, MonasteryBuilding }
    
    public interface ITown
    {
        TownBuildError CanBuildTown();
        ITown Build();
        IPlayer GetIOwner();

        /// <summary>
        /// Controls if is there town already or if another
        /// town is too close.
        /// </summary>
        /// <returns>True if place is free to build town.</returns>
        bool IsPossibleToBuildTown();

        bool BuildSourceBuilding(byte pos);
        IFort BuildFort(byte pos);
        IMonastery BuildMonastery(byte pos);
        IMarket BuildMarket(byte pos);

        int GetTownID();
        ISourceAll GetCost();
        IHexa GetIHexa(byte pos);
        IRoad GetIRoad(byte pos);
        ITown GetITown(byte pos);
        BuildingKind GetBuildingKind(byte pos);
    }
}
