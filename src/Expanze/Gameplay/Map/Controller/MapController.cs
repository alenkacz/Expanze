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

        public MapController(Map map, MapView mapView)
        {
            this.map = map;
            this.mapView = mapView;

            PromptWindow.Inst().setIsActive(false);
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

        public IRoad GetIRoadByID(int roadID)
        {
            if (roadID < 1 || roadID >= roadByID.Length)
                return null;
            if (roadByID[roadID - 1] == null)
                roadByID[roadID - 1] = map.GetRoadByID(roadID);
            return roadByID[roadID - 1];
        }

        public IPlayer GetPlayerMe() { return GameMaster.Inst().getActivePlayer(); }
        public int GetMaxRoadID() { return Road.getRoadCount(); }
        public int GetMaxTownID() { return Town.GetTownCount(); }
        public EGameState GetState() { return GameMaster.Inst().getState(); }
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
            int rate = gm.getActivePlayer().getConversionRate(SourceKindToHexaKind(fromSource));
            if (fromAmount > gm.getActivePlayer().GetSource().Get(fromSource))
                return ChangingSourcesError.NotEnoughFromSource;

            gm.DoMaterialConversion(SourceKindToHexaKind(fromSource), SourceKindToHexaKind(toSource), gm.getActivePlayer(), fromAmount - (fromAmount % rate), fromAmount / rate);

            return ChangingSourcesError.OK;
        }

        public BuyingUpgradeError BuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.Inst();
            Town town = map.GetTownByID(townID);
            if (town == null)
                return BuyingUpgradeError.ThereIsNoTown;
            SpecialBuilding building = town.getSpecialBuilding(hexaID);
            if (building == null)
                return BuyingUpgradeError.ThereIsNoBuilding;

            BuyingUpgradeError error = building.CanActivePlayerBuyUpgrade(upgradeKind, upgradeNumber);
            if (error == BuyingUpgradeError.OK)
            {
                gm.getActivePlayer().PayForSomething(building.getUpgradeCost(upgradeKind, upgradeNumber));
                building.BuyUpgrade(upgradeKind, upgradeNumber);
            }
            return error;
        }

        public RoadBuildError CanBuildRoad(int roadID)
        {
            Road road = map.GetRoadByID(roadID);
            if (road == null)
                return RoadBuildError.InvalidRoadID;
            return road.CanActivePlayerBuildRoad();
        }

        public IRoad BuildRoad(int roadID)
        {
            Road road = map.GetRoadByID(roadID);
            if (road == null)
                return null;

            GameMaster gm = GameMaster.Inst();
            RoadBuildError error = road.CanActivePlayerBuildRoad();
            if (error == RoadBuildError.OK)
            {
                road.BuildRoad(gm.getActivePlayer());

                ItemQueue item = new RoadItemQueue(mapView, roadID);
                mapView.AddToViewQueue(item);

                gm.getActivePlayer().PayForSomething(Settings.costRoad);

                return road;
            }

            return null;
        }

        public BuildingBuildError CanBuildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.Inst();
            Town town = map.GetTownByID(townID);
            if (town == null)
                return BuildingBuildError.InvalidTownID;

            int buildingPos = town.FindBuildingByHexaID(hexaID);
            if (buildingPos == -1)
                return BuildingBuildError.TownHasNoHexaWithThatHexaID;

            HexaModel hexa = town.GetHexa(buildingPos);
            if (hexa.getKind() == HexaKind.Desert && kind == BuildingKind.SourceBuilding)
                return BuildingBuildError.NoSourceBuildingForDesert;

            return town.CanActivePlayerBuildBuildingInTown(buildingPos, kind);
        }

        public bool BuildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.Inst();
            Town town = map.GetTownByID(townID);
            if (town == null)
                return false;

            int buildingPos = town.FindBuildingByHexaID(hexaID);
            if (buildingPos == -1)
                return false;

            HexaModel hexa = town.GetHexa(buildingPos);
            if (hexa.getKind() == HexaKind.Desert && kind == BuildingKind.SourceBuilding)
                return false;

            BuildingBuildError error = town.CanActivePlayerBuildBuildingInTown(buildingPos, kind);
            if (error == BuildingBuildError.OK)
            {
                ItemQueue item = new BuildingItemQueue(mapView, townID, buildingPos);
                mapView.AddToViewQueue(item);

                gm.getActivePlayer().PayForSomething(town.GetBuildingCost(buildingPos, kind));
                town.BuildBuilding(buildingPos, kind);
                return true;
            }

            return false;
        }

        public TownBuildError CanBuildTown(int townID)
        {
            Town town = map.GetTownByID(townID);
            if (town == null)
                return TownBuildError.InvalidTownID;
            return town.CanBuildTown();
        }

        public ITown BuildTown(int townID)
        {
            Town town = map.GetTownByID(townID);
            if (town == null)
                return null;

            GameMaster gm = GameMaster.Inst();
            TownBuildError error = town.CanBuildTown();
            if (error == TownBuildError.OK)
            {
                town.BuildTown(gm.getActivePlayer());

                ItemQueue item = new TownItemQueue(mapView, townID);
                mapView.AddToViewQueue(item);

                if (gm.getState() != EGameState.StateGame)
                {
                    SourceAll source = new SourceAll(0);
                    HexaModel hexa;
                    
                    for (int loop1 = 0; loop1 < 3; loop1++)
                    {
                        if ((hexa = town.GetHexa(loop1)) != null)
                        {
                            switch (hexa.getKind())
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
                            gm.getActivePlayer().AddSources(source, TransactionState.TransactionMiddle);
                        }
                    }
                     
                    gm.NextTurn();
                }
                else
                    gm.getActivePlayer().PayForSomething(Settings.costTown);

                return town;
            }

            return null;
        }

        public void Init()
        {
            townByID = new ITown[Town.GetTownCount()];
            for (int loop1 = 0; loop1 < townByID.Length; loop1++)
                townByID[loop1] = null;
            roadByID = new IRoad[Road.getRoadCount()];
            for (int loop1 = 0; loop1 < roadByID.Length; loop1++)
                roadByID[loop1] = null;
        }
    }
}
