﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum BuildingBuildError { OK, NoSources, AlreadyBuild, NotOwner, TownHasNoHexaWithThatHexaID, NoSourceBuildingForDesert, InvalidTownID, NoBuildingForWater, NoSpecialBuildingForMountains }
    public enum TownBuildError { OK, NoSources, AlreadyBuild, OtherTownIsClose, NoPlayerRoad, InvalidTownID, YouHaveBuiltTownThisTurn }
    public enum BuildingKind { NoBuilding, SourceBuilding, FortBuilding, MarketBuilding, MonasteryBuilding }
    
    public interface ITown
    {
        TownBuildError CanBuildTown();
        ITown Build();

        bool BuildSourceBuilding(byte pos);
        IFort BuildFort(byte pos);
        IMonastery BuildMonastery(byte pos);
        IMarket BuildMarket(byte pos);

        int GetTownID();
        ISourceAll GetCost();
        IHexa GetIHexa(byte pos);
        BuildingKind GetBuildingKind(byte pos);
    }
}
