using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum PriceKind { BRoad, BTown, BMill, BStepherd, BQuarry, BSaw, BMine, BMarket, BMonastery, BFort,
                            UMill1, UStepherd1, UQuarry1, USaw1, UMine1, UMill2, UStepherd2, UQuarry2, USaw2, UMine2,
                            MCorn1, MMeat1, MStone1, MWood1, MOre1, MCorn2, MMeat2, MStone2, MWood2, MOre2,
                            AParade, AStealSources, ACaptureHexa,
                            Medal}
    public enum EGameState { StateFirstTown, StateSecondTown, StateGame, BeforeGame }
    public enum UpgradeKind { NoUpgrade, FirstUpgrade, SecondUpgrade }
    public enum LicenceKind { NoLicence, FirstLicence, SecondLicence }
    public enum SourceBuildingKind { Mill, Stepherd, Quarry, Saw, Mine, Count} // have to be in this order
    public enum PlayerAction { BuildMonastery, BuildMarket, BuildFort, BuySecondLicence, InventSecondUpgrade, FortParade, FortCaptureHexa, FortStealSources }
    public enum PlayerPoints { Town, Road, Market, Fort, Monastery, Quarry, Mine, Stepherd, Mill, Saw, FortParade, FortStealSources, FortCaptureHexa, UpgradeLvl1, UpgradeLvl2, LicenceLvl1, LicenceLvl2, Medal, Count}

    public interface IMapController
    {
        bool IsBanAction(PlayerAction action);
        int GetActionPoints(PlayerPoints action);

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
        bool StealSources(String playerName);

        /// <summary>
        /// Find out if you can destroy other player sources.
        /// </summary>
        /// <param name="playerName">Name of player you want destroy sources.</param>
        /// <returns>Error why you cant destroy sources or DestroySourcesError.OK</returns>
        DestroySourcesError CanStealSources(String playerName);

        /// <summary>
        /// Buy licence in random market for better conversion rate for source kind. 
        /// If you have first licence, you buy second licence. 
        /// Third licence doesn't exist.
        /// </summary>
        /// <param name="source">Kind of source.</param>
        /// <returns>True if licence was bought, otherwise false.</returns>
        bool BuyLicence(SourceKind source);

        MarketError CanBuyLicence(SourceKind source);

        /// <summary>
        /// Invent upgrade for source building of source kind. 
        /// If you have first upgrade, you invent second upgrade. 
        /// Third upgrade doesnt exist.
        /// </summary>
        /// <param name="source">Kind of source building.</param>
        /// <returns>True if upgrade was bought, otherwise false.</returns>
        bool InventUpgrade(SourceBuildingKind source);

        /// <summary>
        /// Returns the reason why you cant invent upgrade or returns
        /// MonasteryError.OK if there is no obstacle to invent upgrade.
        /// </summary>
        /// <param name="source">For which building kind you want invent upgrade.</param>
        /// <returns>MonasteryError.OK if it is OK, or some error.</returns>
        MonasteryError CanInventUpgrade(SourceBuildingKind source);

        bool ChangeSourcesFor(List<ISourceAll> sourceList);
        bool ChangeSourcesFor(ISourceAll source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        int CanChangeSourcesFor(ISourceAll source);

        ParadeError CanShowParade();
        bool ShowParade();

        CaptureHexaError CanCaptureHexa(IHexa hexa);
        bool CaptureHexa(IHexa hexa);

        /// <summary>
        /// Returns price of choosen building, upgrade, licence or action.
        /// </summary>
        /// <param name="kind">Kind of thing you are asking price.</param>
        /// <returns>Price of choosen kind.</returns>
        ISourceAll GetPrice(PriceKind kind);
        ISourceAll GetPrice(SourceKind sourceKind, LicenceKind licenceKind);
        ISourceAll GetPrice(SourceBuildingKind buildingKind, UpgradeKind upgradeKind);

        /// <summary>
        /// How many points you get for each type of building, upgride etc.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns>Number of points</returns>
        int GetPoints(PriceKind kind);

        IPlayer GetPlayerMe();
        List<IPlayer> GetPlayerOthers();
        IHexa GetIHexa(int x, int y);
        IHexa GetIHexaByID(int hexaID);
        ITown GetITownByID(int townID);
        IRoad GetIRoadByID(int roadID);       
        EGameState GetState();
        IGameSetting GetGameSettings();
        int GetTurnNumber();
        String GetLastError();

        int GetDistance(ITown a, ITown b);
        int GetDistanceToTown(ITown town, IPlayer player);
        int GetDistanceToRoad(IRoad road, IPlayer player);
        List<IRoad> GetRoadsToTown(ITown town, IPlayer player);

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

        void Log(string srcFile, string msg);
    }
}
