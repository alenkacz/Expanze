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
        /// <returns>If the building was built</returns>
        bool BuildBuildingInTown(int townID, int hexaID, BuildingKind kind);

        /// <summary>
        /// Change fromAmount amount of one source kind to another according conversion rate of player.
        /// </summary>
        /// <param name="fromSource">Source kind which you dont want to have.</param>
        /// <param name="toSource">Source kind which you want.</param>
        /// <param name="fromAmount">How many pieces of fromSource you want to change.</param>
        /// <returns>Possible error or ChangingSourcesError.OK</returns>
        ChangingSourcesError ChangeSources(SourceKind fromSource, SourceKind toSource, int fromAmount);

        /// <summary>
        /// Destroy half of sources of target player.
        /// You need to have your own fort.
        /// </summary>
        /// <param name="playerName">Name of player you want destroy sources.</param>
        /// <returns>True if destroying sources is succesful, otherwise false</returns>
        bool DestroySources(String playerName);

        IPlayer GetPlayerMe();
        IHexa GetIHexa(int x, int y);
        IHexa GetIHexaByID(int hexaID);
        ITown GetITownByID(int townID);
        IRoad GetIRoadByID(int roadID);       
        EGameState GetState();

        /// <summary>
        /// Min ID is 1 (not 0!)
        /// </summary>
        /// <returns>Max hexa ID</returns>
        int GetMaxHexaID();

        /// <summary>
        /// Min ID is 1 (not 0!)
        /// </summary>
        /// <returns>Max ID of possible town</returns>
        int GetMaxTownID();

        /// <summary>
        /// Min ID is 1 (not 0!)
        /// </summary>
        /// <returns>Max ID of possible road</returns>
        int GetMaxRoadID();
    }
}
