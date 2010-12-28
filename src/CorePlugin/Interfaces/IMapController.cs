using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum EGameState { StateFirstTown, StateSecondTown, StateGame };
    public enum UpgradeKind { NoUpgrade, FirstUpgrade, SecondUpgrade }
    public enum SourceBuildingKind { Mill, Stepherd, Quarry, Saw, Mine, Count} // have to be in this order

    public interface IMapController
    {
        /// <summary>
        /// If it is possible, it builds town on town place with townID.
        /// </summary>
        /// <param name="townID">ID of town place</param>
        /// <returns>Builded town or null if player cant build town.</returns>
        ITown BuildTown(int townID);

        /// <summary>
        /// Find out if player can build town on town place with townID. 
        /// </summary>
        /// <param name="townID">ID of town place</param>
        /// <returns>Reason why it is not possible to build town or TownBuildError.OK if there is no error.</returns>
        TownBuildError CanBuildTown(int townID);

        /// <summary>
        /// If it is possible, it builds road on road place with roadID.
        /// </summary>
        /// <param name="roadID">ID of road place</param>
        /// <returns>Builded road or null if player cant build road.</returns>
        IRoad BuildRoad(int roadID);

        /// <summary>
        /// Find out if player can build road on road place with roadID. 
        /// </summary>
        /// <param name="roadID">ID of road place</param>
        /// <returns>Reason why it is not possible to build road or RoadBuildError.OK if there is no error.</returns>
        RoadBuildError CanBuildRoad(int roadID);

        /// <summary>
        /// Find out if player can build building in town with townID on hexa with hexaID of building kind kind.
        /// </summary>
        /// <param name="townID">ID of town</param>
        /// <param name="hexaID">ID of hexa near town</param>
        /// <param name="kind">kind of building</param>
        /// <returns>Reason why it is not possible to build building int town or BuildingBuildError.OK if there is no error.</returns>
        BuildingBuildError CanBuildBuildingInTown(int townID, int hexaID, BuildingKind kind);

        /// <summary>
        /// If it is possible it builds building in town with townID on hexa with hexaID of building kind kind.
        /// </summary>
        /// <param name="townID">ID of town</param>
        /// <param name="hexaID">ID of hexa near town</param>
        /// <param name="kind">kind of building</param>   
        /// <returns>If the building where builded</returns>
        bool BuildBuildingInTown(int townID, int hexaID, BuildingKind kind);

        BuyingUpgradeError BuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber);
        ChangingSourcesError ChangeSources(SourceKind fromSource, SourceKind toSource, int fromAmount);

        IPlayer GetPlayerMe();
        IHexa GetHexa(int x, int y);
        /// Min ID is 1 (not 0!)
        /// <returns>Max ID of possible town</returns>
        int GetMaxTownID();
        ITown GetITownByID(int townID);
        /// Min ID is 1 (not 0!)
        /// <returns>Max ID of possible road</returns>
        int GetMaxRoadID();
        IRoad GetIRoadByID(int roadID);
        
        EGameState GetState();
    }
}
