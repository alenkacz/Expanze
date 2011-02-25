﻿using System;
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

            PromptWindow.Inst().Deactive();
            MarketComponent.Inst().setIsActive(false);
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
            SpecialBuilding building = town.GetSpecialBuilding(hexaID);
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
            SpecialBuilding building = town.GetSpecialBuilding(hexaID);
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
            if (hexa.GetKind() == HexaKind.Water)
                return BuildingBuildError.NoBuildingForWater;
            if (hexa.GetKind() == HexaKind.Mountains && kind != BuildingKind.SourceBuilding)
                return BuildingBuildError.NoSpecialBuildingForMountains;

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

            BuildingBuildError error = CanBuildBuildingInTown(townID, hexaID, kind);
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
                                case HexaKind.Desert :
                                    source = new SourceAll(20); break;
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

        public bool InventUpgrade(SourceBuildingKind building)
        {
            Player p = GameMaster.Inst().GetActivePlayer();
            List<IMonastery> monastery = p.GetMonastery();

            foreach (IMonastery m in monastery)
            {
                switch (m.CanInventUpgrade(building))
                {
                    case MonasteryError.HaveSecondUpgrade:
                        return false;
                    case MonasteryError.NoSources:
                        return false;
                    case MonasteryError.OK:
                        return m.InventUpgrade(building);
                    case MonasteryError.MaxUpgrades:          // May be free slot in another market
                        break;
                }
            }

            return false;
        }

        public MonasteryError CanInventUpgrade(SourceBuildingKind building)
        {
            Player p = GameMaster.Inst().GetActivePlayer();
            List<IMonastery> monastery = p.GetMonastery();

            foreach (IMonastery m in monastery)
            {
                switch (m.CanInventUpgrade(building))
                {
                    case MonasteryError.HaveSecondUpgrade:
                        return MonasteryError.HaveSecondUpgrade;
                    case MonasteryError.NoSources:
                        return MonasteryError.NoSources;
                    case MonasteryError.OK:
                        return MonasteryError.OK;
                    case MonasteryError.MaxUpgrades:          // May be free slot in another monastery
                        break;
                }
            }

            return MonasteryError.MaxUpgrades;
        }

        public bool BuyLicence(SourceKind source)
        {
            Player p = GameMaster.Inst().GetActivePlayer();
            List<IMarket> market = p.GetMarket();

            foreach (IMarket m in market)
            {
                switch (m.CanBuyLicence(source))
                {
                    case MarketError.HaveSecondLicence:
                        return false;
                    case MarketError.NoSources:
                        return false;
                    case MarketError.OK:
                        return m.BuyLicence(source);
                    case MarketError.MaxLicences:          // May be free slot in another market
                        break;
                }
            }

            return false;
        }

        public MarketError CanBuyLicence(SourceKind source)
        {
            Player p = GameMaster.Inst().GetActivePlayer();
            List<IMarket> market = p.GetMarket();

            foreach (IMarket m in market)
            {
                switch (m.CanBuyLicence(source))
                {
                    case MarketError.HaveSecondLicence :
                        return MarketError.HaveSecondLicence;
                    case MarketError.NoSources :
                        return MarketError.NoSources;
                    case MarketError.OK :
                        return MarketError.OK;
                    case MarketError.MaxLicences :          // May be free slot in another market
                        break;
                }
            }

            return MarketError.MaxLicences;
        }

        public CaptureHexaError CanCaptureHexa(int hexaID, FortModel fort)
        {
            if (!Settings.costFortCapture.HasPlayerSources(GameMaster.Inst().GetActivePlayer()))
                return CaptureHexaError.NoSources;

            if (fort == null)
                return CaptureHexaError.OK;

            HexaModel.SetHexaIDFort(fort.GetHexaID());

            HexaModel hexa = map.GetHexaByID(hexaID);

            if (hexa == null)
                return CaptureHexaError.InvalidHexaID;

            if (!hexa.IsInFortRadius())
                return CaptureHexaError.TooFarFromFort;

            return CaptureHexaError.OK;
        }

        public bool CaptureHexa(int hexaID, FortModel fort)
        {
            if (CanCaptureHexa(hexaID, fort) == CaptureHexaError.OK)
            {
                HexaModel hexa = map.GetHexaByID(hexaID);
                Player player = GameMaster.Inst().GetActivePlayer();
                hexa.Capture(player);
                player.PayForSomething(Settings.costFortCapture);
                return true;
            }
            return false;
        }


        public DestroyHexaError CanDestroyHexa(int hexaID, FortModel fort)
        {
            if (!Settings.costFortDestroyHexa.HasPlayerSources(GameMaster.Inst().GetActivePlayer()))
                return DestroyHexaError.NoSources;

            if (fort == null)
                return DestroyHexaError.OK;

            HexaModel.SetHexaIDFort(fort.GetHexaID());

            HexaModel hexa = map.GetHexaByID(hexaID);

            if (hexa == null)
                return DestroyHexaError.InvalidHexaID;

            if (!hexa.IsInFortRadius())
                return DestroyHexaError.TooFarFromFort;

            return DestroyHexaError.OK;
        }

        public bool DestroyHexa(int hexaID, FortModel fort)
        {
            if (CanDestroyHexa(hexaID, fort) == DestroyHexaError.OK)
            {
                HexaModel hexa = map.GetHexaByID(hexaID);
                hexa.Destroy();
                GameMaster.Inst().GetActivePlayer().PayForSomething(Settings.costFortDestroyHexa);
                return true;
            }
            return false;
        }

        public DestroySourcesError CanStealSources(String playerName)
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

        public bool StealSources(String playerName)
        {
            Player player = GameMaster.Inst().GetPlayer(playerName);
            Player activePlayer = GameMaster.Inst().GetActivePlayer();

            if (CanStealSources(playerName) == DestroySourcesError.OK)
            {
                SourceAll source = (SourceAll) player.GetSource();
                player.PayForSomething(new SourceAll(source / 2));
                activePlayer.AddSources(-Settings.costFortSources, TransactionState.TransactionStart);
                activePlayer.AddSources(source / 2, TransactionState.TransactionEnd);
                return true;
            }
            return false;
        }

        public int CanChangeSources(PriceKind kind)
        {
            SourceAll source = (SourceAll) GetPrice(kind);

            Player player = GameMaster.Inst().GetActivePlayer();

            if (source.HasPlayerSources(player))
                return 0;

            SourceAll delta = (SourceAll) player.GetSource() - source;

            int minusSources = 0;
            int plusSources = 0;
            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (delta[loop1] < 0)
                    minusSources += -delta[loop1];
                else
                    plusSources += delta[loop1] / player.GetConversionRate((SourceKind)loop1);
            }

            return plusSources - minusSources;
        }

        public ISourceAll GetPrice(PriceKind kind)
        {
            switch (kind)
            {
                case PriceKind.BRoad: return Settings.costRoad;
                case PriceKind.BTown: return Settings.costTown;
                case PriceKind.BFort: return Settings.costFort;
                case PriceKind.BMarket: return Settings.costMarket;
                case PriceKind.BMonastery: return Settings.costMonastery;
                case PriceKind.BMill: return Settings.costMill;
                case PriceKind.BStepherd: return Settings.costStephard;
                case PriceKind.BSaw: return Settings.costSaw;
                case PriceKind.BQuarry: return Settings.costQuarry;
                case PriceKind.BMine: return Settings.costMine;
                case PriceKind.UMill1: return Settings.costMonasteryCorn1;
                case PriceKind.UStepherd1: return Settings.costMonasteryMeat1;
                case PriceKind.UQuarry1: return Settings.costMonasteryStone1;
                case PriceKind.USaw1: return Settings.costMonasteryWood1;
                case PriceKind.UMine1: return Settings.costMonasteryOre1;
                case PriceKind.UMill2: return Settings.costMonasteryCorn2;
                case PriceKind.UStepherd2: return Settings.costMonasteryMeat2;
                case PriceKind.UQuarry2: return Settings.costMonasteryStone2;
                case PriceKind.USaw2: return Settings.costMonasteryWood2;
                case PriceKind.UMine2: return Settings.costMonasteryOre2;
                case PriceKind.ICorn1: return Settings.costMarketCorn1;
                case PriceKind.IMeat1: return Settings.costMarketMeat1;
                case PriceKind.IStone1: return Settings.costMarketStone1;
                case PriceKind.IWood1: return Settings.costMarketWood1;
                case PriceKind.IOre1: return Settings.costMarketOre1;
                case PriceKind.ICorn2: return Settings.costMarketCorn2;
                case PriceKind.IMeat2: return Settings.costMarketMeat2;
                case PriceKind.IStone2: return Settings.costMarketStone2;
                case PriceKind.IWood2: return Settings.costMarketWood2;
                case PriceKind.IOre2: return Settings.costMarketOre2;
            }

            return null;
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
