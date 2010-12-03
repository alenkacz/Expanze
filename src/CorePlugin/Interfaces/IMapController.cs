using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum EGameState { StateFirstTown, StateSecondTown, StateGame };
    public enum UpgradeKind { FirstUpgrade, SecondUpgrade }

    public interface IMapController
    {
        IHexaGet GetHexa(int x, int y);
        TownBuildError BuildTown(int townID);
        RoadBuildError BuildRoad(int roadID);
        BuildingBuildError BuildBuildingInTown(int townID, int hexaID, BuildingKind kind);
        void BuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber);
        int GetMaxTownID();
        ITownGet GetITownGetByID(int townID);
        int GetMaxRoadID();
        IRoadGet GetIRoadGetByID(int roadID);
        EGameState GetState();
    }
}
