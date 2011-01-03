using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Expanze.Gameplay.Map.View;

namespace Expanze.Gameplay.Map
{
    class MapController : IMapController
    {
        Map map;
        MapView mapView;

        ITown[] townByID;
        IRoad[] roadByID;
        IHexa[] hexaByID;

        public MapController(Map map, MapView mapView)
        {
            this.map = map;
            this.mapView = mapView;

            PromptWindow.Inst().SetIsActive(false);
            MarketComponent.getInstance().setIsActive(false);
        }

        public ITown GetITownByID(int townID)
        {
            if (townID < 1 || townID >= townByID.Length)
                return null;
            if(townByID[townID - 1] == null)
                townByID[townID - 1] = map.GetTownByID(townID);
            return townByID[townID - 1];
        }

        public IHexa GetIHexaByID(int hexaID)
        {
            if (hexaID < 1 || hexaID >= hexaByID.Length)
                return null;
            if (hexaByID[hexaID - 1] == null)
                hexaByID[hexaID - 1] = map.GetHexaByID(hexaID);
            return hexaByID[hexaID - 1];
        }

        public IRoad GetIRoadByID(int roadID)
        {
            if (roadID < 1 || roadID >= roadByID.Length)
                return null;
            if (roadByID[roadID - 1] == null)
                roadByID[roadID - 1] = map.GetRoadByID(roadID);
            return roadByID[roadID - 1];
        }

        public IPlayer GetPlayerMe() { return GameMaster.Inst().GetActivePlayer(); }
        public int GetMaxRoadID() { return RoadModel.GetRoadCount(); }
        public int GetMaxTownID() { return TownModel.GetTownCount(); }
        public int GetMaxHexaID() { return HexaModel.GetHexaCount(); }
        public EGameState GetState() { return GameMaster.Inst().GetState(); }
        public IHexa GetIHexa(int x, int y) { return map.GetHexaModel(x, y); }

        private HexaKind SourceKindToHexaKind(SourceKind source)
        {
            switch (source)
            {
                case SourceKind.Corn: return HexaKind.Cornfield;
                case SourceKind.Meat: return HexaKind.Pasture;
                case SourceKind.Ore: return HexaKind.Mountains;
                case SourceKind.Stone: return HexaKind.Stone;
                case SourceKind.Wood: return HexaKind.Forest;
                default :
                    return HexaKind.Nothing; // shouldnt happened
            }
        }

        public ChangingSourcesError ChangeSources(SourceKind fromSource, SourceKind toSource, int fromAmount)
        {
            GameMaster gm = GameMaster.Inst();
            int rate = gm.GetActivePlayer().GetConversionRate(fromSource);
            if (fromAmount > gm.GetActivePlayer().GetSource().Get(fromSource))
                return ChangingSourcesError.NotEnoughFromSource;

            gm.DoMaterialConversion(fromSource, toSource, gm.GetActivePlayer(), fromAmount - (fromAmount % rate), fromAmount / rate);

            return ChangingSourcesError.OK;
        }

        public BuyingUpgradeError CanBuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.Inst();
            TownModel town = map.GetTownByID(townID);
            if (town == null)
                return BuyingUpgradeError.ThereIsNoTown;
            SpecialBuilding building = town.getSpecialBuilding(hexaID);
            if (building == null)
                return BuyingUpgradeError.ThereIsNoBuilding;

            return building.CanActivePlayerBuyUpgrade(upgradeKind, upgradeNumber);
        }

        public bool BuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.Inst();
            TownModel town = map.GetTownByID(townID);
            if (town == null)
                return false;
            SpecialBuilding building = town.getSpecialBuilding(hexaID);
            if (building == null)
                return false;

            BuyingUpgradeError error = building.CanActivePlayerBuyUpgrade(upgradeKind, upgradeNumber);
            if (error == BuyingUpgradeError.OK)
            {
                gm.GetActivePlayer().PayForSomething(building.GetUpgradeCost(upgradeKind, upgradeNumber));
                building.BuyUpgrade(upgradeKind, upgradeNumber);

                return true;
            }
            return false;
        }

        public RoadBuildError CanBuildRoad(int roadID)
        {
            RoadModel road = map.GetRoadByID(roadID);
            if (road == null)
                return RoadBuildError.InvalidRoadID;
            return road.CanBuildRoad();
        }

        public IRoad BuildRoad(int roadID)
        {
            RoadModel road = map.GetRoadByID(roadID);
            if (road == null)
                return null;

            GameMaster gm = GameMaster.Inst();
            RoadBuildError error = road.CanBuildRoad();
            if (error == RoadBuildError.OK)
            {
                road.BuildRoad(gm.GetActivePlayer());

                ItemQueue item = new RoadItemQueue(mapView, roadID);
                mapView.AddToViewQueue(item);

                gm.GetActivePlayer().PayForSomething(Settings.costRoad);

                return road;
            }

            return null;
        }

        public BuildingBuildError CanBuildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.Inst();
            TownModel town = map.GetTownByID(townID);
            if (town == null)
                return BuildingBuildError.InvalidTownID;

            int buildingPos = town.FindBuildingByHexaID(hexaID);
            if (buildingPos == -1)
                return BuildingBuildError.TownHasNoHexaWithThatHexaID;

            HexaModel hexa = town.GetHexa(buildingPos);
            if (hexa.GetKind() == HexaKind.Desert && kind == BuildingKind.SourceBuilding)
                return BuildingBuildError.NoSourceBuildingForDesert;

            return town.CanActivePlayerBuildBuildingInTown(buildingPos, kind);
        }

        public bool BuildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.Inst();
            TownModel town = map.GetTownByID(townID);
            if (town == null)
                return false;

            int buildingPos = town.FindBuildingByHexaID(hexaID);
            if (buildingPos == -1)
                return false;

            HexaModel hexa = town.GetHexa(buildingPos);
            if (hexa.GetKind() == HexaKind.Desert && kind == BuildingKind.SourceBuilding)
                return false;

            BuildingBuildError error = town.CanActivePlayerBuildBuildingInTown(buildingPos, kind);
            if (error == BuildingBuildError.OK)
            {
                ItemQueue item = new BuildingItemQueue(mapView, townID, buildingPos);
                mapView.AddToViewQueue(item);

                gm.GetActivePlayer().PayForSomething(town.GetBuildingCost(buildingPos, kind));
                town.BuildBuilding(buildingPos, kind);
                return true;
            }

            return false;
        }

        public TownBuildError CanBuildTown(int townID)
        {
            TownModel town = map.GetTownByID(townID);
            if (town == null)
                return TownBuildError.InvalidTownID;
            return town.CanBuildTown();
        }

        public ITown BuildTown(int townID)
        {
            TownModel town = map.GetTownByID(townID);
            if (town == null)
                return null;

            GameMaster gm = GameMaster.Inst();
            TownBuildError error = town.CanBuildTown();
            if (error == TownBuildError.OK)
            {
                town.BuildTown(gm.GetActivePlayer());

                ItemQueue item = new TownItemQueue(mapView, townID);
                mapView.AddToViewQueue(item);

                if (gm.GetState() != EGameState.StateGame)
                {
                    SourceAll source = new SourceAll(0);
                    HexaModel hexa;
                    
                    for (int loop1 = 0; loop1 < 3; loop1++)
                    {
                        if ((hexa = town.GetHexa(loop1)) != null)
                        {
                            switch (hexa.GetKind())
                            {
                                case HexaKind.Cornfield:
                                    source = Settings.costMill; break;
                                case HexaKind.Forest:
                                    source = Settings.costSaw; break;
                                case HexaKind.Mountains:
                                    source = Settings.costMine; break;
                                case HexaKind.Pasture:
                                    source = Settings.costStephard; break;
                                case HexaKind.Stone:
                                    source = Settings.costQuarry; break;
                                default:
                                    source = new SourceAll(0); break;
                            }
                            gm.GetActivePlayer().AddSources(source, TransactionState.TransactionMiddle);
                        }
                    }

                    gm.SetHasBuiltTown(true);

                    if(!gm.GetActivePlayer().GetIsAI())
                        gm.NextTurn();                   
                }
                else
                    gm.GetActivePlayer().PayForSomething(Settings.costTown);

                return town;
            }

            return null;
        }

        public CaptureHexaError CanCaptureHexa(int hexaID, FortModel fort)
        {
            if (!Settings.costFortCapture.HasPlayerSources(GameMaster.Inst().GetActivePlayer()))
                return CaptureHexaError.NoSources;

            if (fort == null)
                return CaptureHexaError.OK;

            HexaModel.SetHexaIDFort(fort.GetHexaID());

            return CaptureHexaError.OK;
        }

        public bool CaptureHexa(int hexaID, FortModel fort)
        {
            HexaModel hexa = map.GetHexaByID(hexaID);
            hexa.Capture();
            GameMaster.Inst().GetActivePlayer().PayForSomething(Settings.costFortCapture);

            return true;
        }

        public bool DestroyHexa(int hexaID)
        {
            HexaModel hexa = map.GetHexaByID(hexaID);
            hexa.Destroy();
            GameMaster.Inst().GetActivePlayer().PayForSomething(Settings.costFortDestroyHexa);
            return true;
        }

        public DestroySourcesError CanDestroySources(String playerName)
        {
            Player player = GameMaster.Inst().GetPlayer(playerName);
            if (player == null)
                return DestroySourcesError.NoPlayerWithName;

            Player activePlayer = GameMaster.Inst().GetActivePlayer();
            if (activePlayer.GetBuildingCount(Building.Fort) == 0)
                return DestroySourcesError.NoFort;
            if (!Settings.costFortSources.HasPlayerSources(activePlayer))
                return DestroySourcesError.NoSources;

            return DestroySourcesError.OK;
        }

        public bool DestroySources(String playerName)
        {
            Player player = GameMaster.Inst().GetPlayer(playerName);
            Player activePlayer = GameMaster.Inst().GetActivePlayer();

            if (CanDestroySources(playerName) == DestroySourcesError.OK)
            {
                ISourceAll source = player.GetSource();
                player.PayForSomething(new SourceAll(source.GetWood() / 2, source.GetStone() / 2, source.GetCorn() / 2, source.GetMeat() / 2, source.GetOre() / 2));
                activePlayer.PayForSomething(Settings.costFortSources);
                return true;
            }
            return false;
        }

        public void Init()
        {
            townByID = new ITown[TownModel.GetTownCount()];
            for (int loop1 = 0; loop1 < townByID.Length; loop1++)
                townByID[loop1] = null;
            roadByID = new IRoad[RoadModel.GetRoadCount()];
            for (int loop1 = 0; loop1 < roadByID.Length; loop1++)
                roadByID[loop1] = null;
            hexaByID = new IHexa[HexaModel.GetHexaCount()];
            for (int loop1 = 0; loop1 < hexaByID.Length; loop1++)
                hexaByID[loop1] = null;

        }
    }
}
