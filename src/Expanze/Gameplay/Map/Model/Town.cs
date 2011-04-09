using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CorePlugin;
using Expanze.Gameplay;

namespace Expanze
{
    class TownModel : ITown
    {
        private Player playerOwner; /// owner of this town, if null no player owns town on this place
        private bool isBuild;       /// is there town or it is only place for possible town
        private RoadModel[] roadNeighbour; /// two or 3 neighbours
        private TownModel[] townNeighbour; /// two or 3 neighbours
        private HexaModel[] hexaNeighbour; /// 1 - 3 neighbours
        private BuildingKind[] buildingKind; /// indeces corresponds with hexaNeighbour indeces
        private SpecialBuilding[] building; /// indeces corresponds with hexaNeighbour indeces

        private int townID;             /// unique ID of town place, from 1 to counter
        public static int counter = 0;  /// how many town places are on the map
        private PathNode pathNode;      /// node used for finding path to town

        public int GetTownID() { return townID; }
        public bool GetIsBuild() { return isBuild; }
        public Player GetOwner() { return playerOwner; }
        public IPlayer GetIOwner() { return playerOwner; }
        public ISourceAll GetCost() { return Settings.costTown; }
        public IHexa GetIHexa(byte pos) { return GetHexa(pos); }
        public IRoad GetIRoad(byte pos) { return roadNeighbour[pos]; }
        public HexaModel GetHexa(int pos) { return hexaNeighbour[pos]; }
        public ITown GetITown(byte pos) { return townNeighbour[pos]; }
        public TownModel GetTown(byte pos) { return townNeighbour[pos]; }
        public static int GetTownCount() { return counter; }    // number of towns

        public TownModel()
        {
            townID = ++counter;
            isBuild = false;

            roadNeighbour = new RoadModel[3];
            townNeighbour = new TownModel[3];
            hexaNeighbour = new HexaModel[3];
            buildingKind = new BuildingKind[3];
            building = new SpecialBuilding[3];
            pathNode = new PathNode();

            for (int loop1 = 0; loop1 < buildingKind.Length; loop1++)
                buildingKind[loop1] = BuildingKind.NoBuilding;

            //building[1] = BuildingKind.SourceBuilding;

            playerOwner = null;
        }

        /// <summary>
        /// It checks hexa ID of neighbours and return position of possible building.
        /// </summary>
        /// <param name="hexaID">ID of hexa</param>
        /// <returns>0-2 position, -1 if town has no neigbour with hexaID</returns>
        public int FindBuildingByHexaID(int hexaID)
        {
            for (int loop1 = 0; loop1 < buildingKind.Length; loop1++)
            {
                if (hexaNeighbour[loop1] != null && hexaNeighbour[loop1].GetID() == hexaID)
                    return loop1;
            }
            return -1;
        }

        public BuildingKind GetBuildingKind(byte pos)
        {
            if (pos >= 0 && pos <= 2)
                return buildingKind[pos];
            else
                return BuildingKind.NoBuilding;
        }

        public BuildingKind GetBuildingKind(int hexaID)
        {
            if (!isBuild)
                return BuildingKind.NoBuilding;

            int buildingPos = FindBuildingByHexaID(hexaID);

            return (buildingPos == -1) ? BuildingKind.NoBuilding : buildingKind[buildingPos];
        }

        public SpecialBuilding GetSpecialBuilding(int hexaID)
        {
            if (!isBuild)
                return null;

            int buildingPos = FindBuildingByHexaID(hexaID);

            return (buildingPos == -1) ? null : building[buildingPos];
        }

        public static void ResetCounter() { counter = 0; }

        public void SetRoadNeighbours(RoadModel road1, RoadModel road2, RoadModel road3)
        {
            roadNeighbour[0] = road1;
            roadNeighbour[1] = road2;
            roadNeighbour[2] = road3;
        }

        public void SetTownNeighbours(TownModel town1, TownModel town2, TownModel town3)
        {
            townNeighbour[0] = town1;
            townNeighbour[1] = town2;
            townNeighbour[2] = town3;
        }

        public void SetHexaNeighbours(HexaModel hexa1, HexaModel hexa2, HexaModel hexa3)
        {
            hexaNeighbour[0] = hexa1;
            hexaNeighbour[1] = hexa2;
            hexaNeighbour[2] = hexa3;
        }

        public void CollectSources(Player player)
        {
            SourceAll sourceNow = new SourceAll(0);
            SourceAll sourceNormal = new SourceAll(0);
            int amountNow;
            int amountNormal;

            for(int loop1 = 0; loop1 < 3; loop1++)
            {
                if (hexaNeighbour[loop1] != null &&
                    buildingKind[loop1] == BuildingKind.SourceBuilding)
                {
                    SourceBuildingModel tempBuilding = (SourceBuildingModel) building[loop1];
                    float multiply = (tempBuilding.GetUpgrade() == UpgradeKind.SecondUpgrade) ? 2.0f : (tempBuilding.GetUpgrade() == UpgradeKind.FirstUpgrade) ? 1.5f : 1.0f;
                    
                    amountNormal = hexaNeighbour[loop1].GetCurrentSource();
                    amountNow = (int)(amountNormal * multiply);
                    
                    if (player == hexaNeighbour[loop1].GetCapturedPlayer() ||
                        (player == playerOwner && !hexaNeighbour[loop1].GetCaptured()))
                    {
                        switch (hexaNeighbour[loop1].GetKind())
                        {
                            case HexaKind.Forest:
                                sourceNow    += new SourceAll(0, 0, 0, amountNow, 0);
                                sourceNormal += new SourceAll(0, 0, 0, amountNormal, 0);
                                break;

                            case HexaKind.Stone:
                                sourceNow    += new SourceAll(0, 0, amountNow, 0, 0);
                                sourceNormal += new SourceAll(0, 0, amountNormal, 0, 0);
                                break;

                            case HexaKind.Cornfield:
                                sourceNow    += new SourceAll(amountNow, 0, 0, 0, 0);
                                sourceNormal += new SourceAll(amountNormal, 0, 0, 0, 0);
                                break;

                            case HexaKind.Pasture:
                                sourceNow    += new SourceAll(0, amountNow, 0, 0, 0);
                                sourceNormal += new SourceAll(0, amountNormal, 0, 0, 0);
                                break;

                            case HexaKind.Mountains:
                                sourceNow    += new SourceAll(0, 0, 0, 0, amountNow);
                                sourceNormal += new SourceAll(0, 0, 0, 0, amountNormal);
                                break;
                        }
                    }
                }
            }
            player.AddSources(sourceNow, TransactionState.TransactionMiddle);
            player.AddCollectSources(sourceNormal, sourceNow);
        }

        public void BuildTown(Player player)
        {
            playerOwner = player;
            player.AddPoints(Settings.pointsTown);
            player.AddBuilding(Building.Town);
            player.AddTown(this);
            isBuild = true;
        }

        /// <summary>
        /// It builds town on this position. Uses by AI.
        /// </summary>
        /// <returns>Itself or null if town cant be builded.</returns>
        public ITown Build()
        {
            return GameState.map.GetMapController().BuildTown(townID);
        }

        public Boolean HasPlayerRoadNeighbour(Player player)
        {
            foreach (RoadModel road in roadNeighbour)
            {
                if (road != null)
                {
                    if (road.GetOwner() == player)
                        return true;
                }
            }
            return false;
        }

        // has someone already built town next to this spot?
        public Boolean HasTownBuildNeighbour()
        {
            for (int loop1 = 0; loop1 < townNeighbour.Length; loop1++)
            {
                if (townNeighbour[loop1] != null)
                {
                    if (townNeighbour[loop1].GetIsBuild())
                        return true;
                }
            }

            return false;
        }

        public SourceAll GetBuildingCost(int pos, BuildingKind kind)
        {
            SourceAll cost = new SourceAll(0);
            switch (kind)
            {
                case BuildingKind.SourceBuilding:
                    cost = (SourceAll) hexaNeighbour[pos].GetSourceBuildingCost();
                    break;

                case BuildingKind.FortBuilding:
                    cost = Settings.costFort;
                    break;

                case BuildingKind.MarketBuilding:
                    cost = Settings.costMarket;
                    break;

                case BuildingKind.MonasteryBuilding:
                    cost = Settings.costMonastery;
                    break;
            }
            return cost;
        }

        public BuildingBuildError CanActivePlayerBuildBuildingInTown(int pos, BuildingKind kind)
        {
            GameMaster gm = GameMaster.Inst();

            SourceAll cost = GetBuildingCost(pos, kind);        
            
            if (gm.GetState() == EGameState.StateGame)
            {
                if (buildingKind[pos] != BuildingKind.NoBuilding)
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_ALREADY_BUILD);
                    return BuildingBuildError.AlreadyBuild;
                }

                if (gm.GetActivePlayer() != playerOwner)
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_NO_OWNER);
                    return BuildingBuildError.NotOwner;
                }

                if (!cost.HasPlayerSources(gm.GetActivePlayer()))
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_NO_SOURCES);
                    return BuildingBuildError.NoSources;
                }

                if (cost == new SourceAll(0))
                {
                    // WHAT?
                }

                return BuildingBuildError.OK;
            }

            return BuildingBuildError.NoSources;
        }

        public bool IsPossibleToBuildTown()
        {
            if (isBuild)
                return false;
            if (HasTownBuildNeighbour())
                return false;

            return true;
        }

        public TownBuildError CanBuildTown()
        {
            GameMaster gm = GameMaster.Inst();
            if (gm.GetState() == EGameState.StateGame)
            {
                Player activePlayer = gm.GetActivePlayer();
                Boolean hasActivePlayerRoadNeighbour = false;

                if (isBuild)
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_ALREADY_BUILD);
                    return TownBuildError.AlreadyBuild;
                }
                if (HasTownBuildNeighbour())
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_OTHER_TOWN_IS_TOO_CLOSE);
                    return TownBuildError.OtherTownIsClose;
                }

                foreach (RoadModel road in roadNeighbour)
                {
                    if (road != null && road.GetOwner() == activePlayer)
                        hasActivePlayerRoadNeighbour = true;
                }

                if (!hasActivePlayerRoadNeighbour)
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_NO_PLAYER_ROAD);
                    return TownBuildError.NoPlayerRoad;
                }

                if (!Settings.costTown.HasPlayerSources(activePlayer))
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_NO_SOURCES);
                    return TownBuildError.NoSources;
                }

                return TownBuildError.OK;
            }
            else
            {
                if (gm.GetHasBuiltTown())
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_YOU_HAVE_BUILT_TOWN_THIS_TURN);
                    return TownBuildError.YouHaveBuiltTownThisTurn;
                }
                if (isBuild)
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_ALREADY_BUILD);
                    return TownBuildError.AlreadyBuild;
                }
                if (HasTownBuildNeighbour())
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_OTHER_TOWN_IS_TOO_CLOSE);
                    return TownBuildError.OtherTownIsClose;
                }

                return TownBuildError.OK;
            }
        }

        public ISpecialBuildingGet BuildBuilding(int pos, BuildingKind kind)
        {
            buildingKind[pos] = kind;
            switch (kind)
            {
                case BuildingKind.SourceBuilding :
                    building[pos] = new SourceBuildingModel(playerOwner, townID, hexaNeighbour[pos].GetID());

                    switch (hexaNeighbour[pos].GetKind())
                    {
                        case HexaKind.Cornfield: playerOwner.AddBuilding(Building.Mill); break;
                        case HexaKind.Pasture: playerOwner.AddBuilding(Building.Stepherd); break;
                        case HexaKind.Stone: playerOwner.AddBuilding(Building.Quarry); break;
                        case HexaKind.Forest: playerOwner.AddBuilding(Building.Saw); break;
                        case HexaKind.Mountains: playerOwner.AddBuilding(Building.Mine); break;
                    }
                    break;

                case BuildingKind.MarketBuilding :
                    MarketModel marketModel = new MarketModel(playerOwner, townID, hexaNeighbour[pos].GetID());
                    building[pos] = marketModel;
                    playerOwner.AddBuilding(Building.Market);
                    playerOwner.AddPoints(Settings.pointsMarket);
                    playerOwner.AddMarket(marketModel);
                    break;
                case BuildingKind.MonasteryBuilding :
                    MonasteryModel monasteryModel = new MonasteryModel(playerOwner, townID, hexaNeighbour[pos].GetID());
                    building[pos] = monasteryModel;
                    playerOwner.AddBuilding(Building.Monastery);
                    playerOwner.AddPoints(Settings.pointsMonastery);
                    playerOwner.AddMonastery(monasteryModel);
                    break;
                case BuildingKind.FortBuilding :
                    FortModel fortModel = new FortModel(playerOwner, townID, hexaNeighbour[pos].GetID());
                    building[pos] = fortModel;
                    playerOwner.AddBuilding(Building.Fort);
                    playerOwner.AddPoints(Settings.pointsFort);
                    playerOwner.AddFort(fortModel);
                    break;
            }

            return building[pos];
        }

        public bool BuildSourceBuilding(byte pos)
        {
            if (pos < 0 || pos > 2)
                return false;

            if (GameState.map.GetMapController().BuildBuildingInTown(townID, hexaNeighbour[pos].GetID(), BuildingKind.SourceBuilding))
                return true;
            else
                return false;
        }

        public IFort BuildFort(byte pos)
        {
            if (pos < 0 || pos > 2)
                return null;

            if(GameState.map.GetMapController().BuildBuildingInTown(townID, hexaNeighbour[pos].GetID(), BuildingKind.FortBuilding))
                return (IFort)building[pos];
            else
                return null;      
        }

        public IMonastery BuildMonastery(byte pos)
        {
            if (pos < 0 || pos > 2)
                return null;

            if (GameState.map.GetMapController().BuildBuildingInTown(townID, hexaNeighbour[pos].GetID(), BuildingKind.MonasteryBuilding))
                return (IMonastery)building[pos];
            else
                return null;
        }

        public IMarket BuildMarket(byte pos)
        {
            if (pos < 0 || pos > 2)
                return null;

            if (GameState.map.GetMapController().BuildBuildingInTown(townID, hexaNeighbour[pos].GetID(), BuildingKind.MarketBuilding))
                return (IMarket)building[pos];
            else
                return null;
        }

        internal void ClearNodePath()
        {
            pathNode.Clear();
        }

        internal PathNode GetPathNode() { return pathNode; }
        internal void SetPathNode(int distance, TownModel ancestorTown, IRoad ancestorRoad)
        {
            pathNode.Set(distance, ancestorTown, ancestorRoad);
        }
    }
}