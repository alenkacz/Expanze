using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum EGameState { StateFirstTown, StateSecondTown, StateGame };
    public enum UpgradeKind { NoUpgrade, FirstUpgrade, SecondUpgrade }
    public enum SourceBuildingKind { Mill, Mine, Quarry, Stepherd, Saw, Count}

    public interface IMapController
    {
        TownBuildError BuildTown(int townID);
        RoadBuildError BuildRoad(int roadID);
        BuildingBuildError BuildBuildingInTown(int townID, int hexaID, BuildingKind kind);
        BuyingUpgradeError BuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber);
        ChangingSourcesError ChangeSources(SourceKind fromSource, SourceKind toSource, int fromAmount);

        IPlayerGet GetPlayerMe();
        IHexaGet GetHexa(int x, int y);
        /// Min ID is 1 (not 0!)
        /// <returns>Max ID of possible town</returns>
        int GetMaxTownID();
        ITownGet GetITownGetByID(int townID);
        /// Min ID is 1 (not 0!)
        /// <returns>Max ID of possible road</returns>
        int GetMaxRoadID();
        IRoadGet GetIRoadGetByID(int roadID);
        
        EGameState GetState();
    }
}
