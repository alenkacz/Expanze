﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze.Gameplay.Map
{
    class MapController : IMapController
    {
        Map map;
        MapView mapView;

        public MapController(Map map, MapView mapView)
        {
            this.map = map;
            this.mapView = mapView;
        }

        public ITownGet GetITownGetByID(int townID)
        {
            return map.GetTownByID(townID);
        }

        public IRoadGet GetIRoadGetByID(int roadID)
        {
            return map.GetRoadByID(roadID);
        }

        public int GetMaxRoadID() { return Road.getRoadCount(); }
        public int GetMaxTownID() { return Town.getTownCount(); }
        public EGameState GetState() { return GameMaster.getInstance().getState(); }

        public IHexaGet GetHexa(int x, int y)
        {
            return map.GetHexaModel(x, y);
        }

        public void BuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.getInstance();
            Town town = map.GetTownByID(townID);
            SpecialBuilding building = town.getSpecialBuilding(hexaID);
            building.BuyUpgrade(upgradeKind, upgradeNumber);
            gm.getActivePlayer().payForSomething(building.getUpgradeCost(upgradeKind, upgradeNumber));
        }

        public RoadBuildError BuildRoad(int roadID)
        {
            Road road = map.GetRoadByID(roadID);

            GameMaster gm = GameMaster.getInstance();
            RoadBuildError error = road.CanActivePlayerBuildRoad();
            if (error == RoadBuildError.OK)
            {
                road.BuildRoad(gm.getActivePlayer());

                ItemQueue item = new ItemQueue(ItemKind.BuildRoad, roadID);
                mapView.AddToViewQueue(item);

                gm.getActivePlayer().payForSomething(Settings.costRoad);
            }

            return error;
        }

        public BuildingBuildError BuildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.getInstance();
            Town town = map.GetTownByID(townID);
            int buildingPos = town.findBuildingByHexaID(hexaID);
            HexaModel hexa = town.getHexa(buildingPos);

            SourceAll buildingCost = new SourceAll(0);
            switch (kind)
            {
                case BuildingKind.SourceBuilding:
                    buildingCost = hexa.getSourceBuildingCost();
                    break;

                case BuildingKind.FortBuilding:
                    buildingCost = Settings.costFort;
                    break;
            }

            BuildingBuildError error = town.canActivePlayerBuildBuildingInTown(buildingPos, buildingCost);
            if (error == BuildingBuildError.OK)
            {
                town.buildBuilding(buildingPos, kind);
                gm.getActivePlayer().payForSomething(buildingCost);
            }

            return error;
        }

        public TownBuildError BuildTown(int townID)
        {
            Town town = map.GetTownByID(townID);

            GameMaster gm = GameMaster.getInstance();
            TownBuildError error = town.CanActivePlayerBuildTown();
            if (error == TownBuildError.OK)
            {
                town.BuildTown(gm.getActivePlayer());

                ItemQueue item = new ItemQueue(ItemKind.BuildTown, townID);
                mapView.AddToViewQueue(item);

                if (gm.getState() != EGameState.StateGame)
                {
                    SourceAll source = new SourceAll(0);
                    HexaModel hexa;
                    gm.getActivePlayer().addSources(new SourceAll(0), TransactionState.TransactionStart);
                    for (int loop1 = 0; loop1 < 3; loop1++)
                    {
                        if ((hexa = town.getHexa(loop1)) != null)
                        {
                            switch (hexa.getType())
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
                    gm.getActivePlayer().addSources(new SourceAll(0), TransactionState.TransactionEnd);

                    if (!gm.getActivePlayer().getIsAI())
                        gm.NextTurn();
                }
                else
                    gm.getActivePlayer().payForSomething(Settings.costTown);
            }

            return error;
        }
    }
}
