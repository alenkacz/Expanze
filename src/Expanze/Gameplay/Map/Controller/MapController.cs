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

        ITownGet[] townByID;
        IRoadGet[] roadByID;

        public MapController(Map map, MapView mapView)
        {
            this.map = map;
            this.mapView = mapView;

            PromptWindow.Inst().setIsActive(false);
            MarketComponent.getInstance().setIsActive(false);
        }

        public ITownGet GetITownGetByID(int townID)
        {
            if (townID < 1 || townID >= townByID.Length)
                return null;
            if(townByID[townID - 1] == null)
                townByID[townID - 1] = map.GetTownByID(townID);
            return townByID[townID - 1];
        }

        public IRoadGet GetIRoadGetByID(int roadID)
        {
            if (roadID < 1 || roadID >= roadByID.Length)
                return null;
            if (roadByID[roadID - 1] == null)
                roadByID[roadID - 1] = map.GetRoadByID(roadID);
            return roadByID[roadID - 1];
        }

        public IPlayerGet GetPlayerMe() { return GameMaster.getInstance().getActivePlayer(); }
        public int GetMaxRoadID() { return Road.getRoadCount(); }
        public int GetMaxTownID() { return Town.getTownCount(); }
        public EGameState GetState() { return GameMaster.getInstance().getState(); }
        public IHexaGet GetHexa(int x, int y) { return map.GetHexaModel(x, y); }

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
            GameMaster gm = GameMaster.getInstance();
            int rate = gm.getActivePlayer().getConversionRate(SourceKindToHexaKind(fromSource));
            if (fromAmount > gm.getActivePlayer().GetSource().Get(fromSource))
                return ChangingSourcesError.NotEnoughFromSource;

            gm.doMaterialConversion(SourceKindToHexaKind(fromSource), SourceKindToHexaKind(toSource), gm.getActivePlayer(), fromAmount - (fromAmount % rate), fromAmount / rate);

            return ChangingSourcesError.OK;
        }

        public BuyingUpgradeError BuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.getInstance();
            Town town = map.GetTownByID(townID);
            if (town == null)
                return BuyingUpgradeError.ThereIsNoTown;
            SpecialBuilding building = town.getSpecialBuilding(hexaID);
            if (building == null)
                return BuyingUpgradeError.ThereIsNoBuilding;

            BuyingUpgradeError error = building.CanActivePlayerBuyUpgrade(upgradeKind, upgradeNumber);
            if (error == BuyingUpgradeError.OK)
            {
                gm.getActivePlayer().payForSomething(building.getUpgradeCost(upgradeKind, upgradeNumber));
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

        public IRoadGet BuildRoad(int roadID)
        {
            Road road = map.GetRoadByID(roadID);
            if (road == null)
                return null;

            GameMaster gm = GameMaster.getInstance();
            RoadBuildError error = road.CanActivePlayerBuildRoad();
            if (error == RoadBuildError.OK)
            {
                road.BuildRoad(gm.getActivePlayer());

                ItemQueue item = new RoadItemQueue(mapView, roadID);
                mapView.AddToViewQueue(item);

                gm.getActivePlayer().payForSomething(Settings.costRoad);

                return road;
            }

            return null;
        }

        public BuildingBuildError CanBuildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.getInstance();
            Town town = map.GetTownByID(townID);
            if (town == null)
                return BuildingBuildError.InvalidTownID;

            int buildingPos = town.findBuildingByHexaID(hexaID);
            if (buildingPos == -1)
                return BuildingBuildError.TownHasNoHexaWithThatHexaID;

            HexaModel hexa = town.getHexa(buildingPos);
            if (hexa.getKind() == HexaKind.Desert && kind == BuildingKind.SourceBuilding)
                return BuildingBuildError.NoSourceBuildingForDesert;

            return town.canActivePlayerBuildBuildingInTown(buildingPos, kind);
        }

        public ISpecialBuildingGet BuildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.getInstance();
            Town town = map.GetTownByID(townID);
            if (town == null)
                return null;

            int buildingPos = town.findBuildingByHexaID(hexaID);
            if (buildingPos == -1)
                return null;

            HexaModel hexa = town.getHexa(buildingPos);
            if (hexa.getKind() == HexaKind.Desert && kind == BuildingKind.SourceBuilding)
                return null;

            BuildingBuildError error = town.canActivePlayerBuildBuildingInTown(buildingPos, kind);
            if (error == BuildingBuildError.OK)
            {
                ItemQueue item = new BuildingItemQueue(mapView, townID, buildingPos);
                mapView.AddToViewQueue(item);

                gm.getActivePlayer().payForSomething(town.GetBuildingCost(buildingPos, kind));
                return town.BuildBuilding(buildingPos, kind);
            }

            return null;
        }

        public TownBuildError CanBuildTown(int townID)
        {
            Town town = map.GetTownByID(townID);
            if (town == null)
                return TownBuildError.InvalidTownID;
            return town.CanActivePlayerBuildTown();
        }

        public ITownGet BuildTown(int townID)
        {
            Town town = map.GetTownByID(townID);
            if (town == null)
                return null;

            GameMaster gm = GameMaster.getInstance();
            TownBuildError error = town.CanActivePlayerBuildTown();
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
                        if ((hexa = town.getHexa(loop1)) != null)
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
                            gm.getActivePlayer().addSources(source, TransactionState.TransactionMiddle);
                        }
                    }
                    

                    //if (!gm.getActivePlayer().getIsAI())
                        gm.NextTurn();
                }
                else
                    gm.getActivePlayer().payForSomething(Settings.costTown);

                return town;
            }

            return null;
        }

        public void Init()
        {
            townByID = new ITownGet[Town.getTownCount()];
            for (int loop1 = 0; loop1 < townByID.Length; loop1++)
                townByID[loop1] = null;
            roadByID = new IRoadGet[Road.getRoadCount()];
            for (int loop1 = 0; loop1 < roadByID.Length; loop1++)
                roadByID[loop1] = null;
        }
    }
}
