using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Expanze.Gameplay.Map.View;
using Expanze.Utils;

namespace Expanze.Gameplay.Map
{
    class MapController : IMapController
    {
        Map map;
        MapView mapView;
        String lastError;
        GameMaster gm;

        ITown[] townByID;
        IRoad[] roadByID;
        IHexa[] hexaByID;

        public MapController(Map map, MapView mapView)
        {
            this.map = map;
            this.mapView = mapView;
            this.gm = GameMaster.Inst();

            PromptWindow.Inst().Deactive();
            MarketComponent.Inst().SetIsActive(false);
        }

        public ITown GetITownByID(int townID)
        {
            if (townID < 1 || townID > townByID.Length)
                return null;
            if(townByID[townID - 1] == null)
                townByID[townID - 1] = map.GetTownByID(townID);
            return townByID[townID - 1];
        }

        public IHexa GetIHexaByID(int hexaID)
        {
            if (hexaID < 1 || hexaID > hexaByID.Length)
                return null;
            if (hexaByID[hexaID - 1] == null)
                hexaByID[hexaID - 1] = map.GetHexaByID(hexaID);
            return hexaByID[hexaID - 1];
        }

        public IRoad GetIRoadByID(int roadID)
        {
            if (roadID < 1 || roadID > roadByID.Length)
                return null;
            if (roadByID[roadID - 1] == null)
                roadByID[roadID - 1] = map.GetRoadByID(roadID);
            return roadByID[roadID - 1];
        }

        public IPlayer GetPlayerMe() { return gm.GetActivePlayer(); }

        public List<IPlayer> GetPlayerOthers()
        {
            List<IPlayer> players = new List<IPlayer>();

            foreach (IPlayer p in gm.GetPlayers())
            {
                if (p != GetPlayerMe())
                {
                    players.Add(p);
                }
            }

            return players;
        }

        public int GetMaxRoadID() { return RoadModel.GetRoadCount(); }
        public int GetMaxTownID() { return TownModel.GetTownCount(); }
        public int GetMaxHexaID() { return HexaModel.GetHexaCount(); }
        public EGameState GetState() { return gm.GetState(); }
        public IHexa GetIHexa(int x, int y) { return map.GetHexaModel(x, y); }

        public void SetLastError(String str) {lastError = str;}
        public String GetLastError() { return lastError; }

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
            {
                SetLastError(Strings.ERROR_NOT_ENOUGHT_FROM_SOURCE);
                return ChangingSourcesError.NotEnoughFromSource;
            }

            gm.DoMaterialConversion(fromSource, toSource, gm.GetActivePlayer(), fromAmount - (fromAmount % rate), fromAmount / rate);

            return ChangingSourcesError.OK;
        }

        public BuyingUpgradeError CanBuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.Inst();
            TownModel town = map.GetTownByID(townID);
            if (town == null)
            {
                SetLastError(Strings.ERROR_THERE_IS_NO_TOWN);
                return BuyingUpgradeError.ThereIsNoTown;
            }
            SpecialBuilding building = town.GetSpecialBuilding(hexaID);
            if (building == null)
            {
                SetLastError(Strings.ERROR_THERE_IS_NO_BUILDING);
                return BuyingUpgradeError.ThereIsNoBuilding;
            }

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
            {
                SetLastError(Strings.ERROR_INVALID_ROAD_ID);
                return RoadBuildError.InvalidRoadID;
            }
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
                PathNode.SetIsValid(false);
                road.BuildRoad(gm.GetActivePlayer());

                ItemQueue item = new RoadItemQueue(mapView, roadID);
                mapView.AddToViewQueue(item);

                if(gm.GetState() != EGameState.BeforeGame)
                    gm.GetActivePlayer().PayForSomething(Settings.costRoad);

                TriggerManager.Inst().TurnTrigger(TriggerType.RoadBuild, roadID);
                return road;
            }

            return null;
        }

        public BuildingBuildError CanBuildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.Inst();
            if (gm.GetState() == EGameState.BeforeGame)
                return BuildingBuildError.OK;

            if (kind == BuildingKind.FortBuilding && Settings.banFort)
            {
                SetLastError(Strings.ERROR_BAN_FORT);
                return BuildingBuildError.Ban;
            }

            if (kind == BuildingKind.MarketBuilding && Settings.banMarket)
            {
                SetLastError(Strings.ERROR_BAN_MARKET);
                return BuildingBuildError.Ban;
            }

            if (kind == BuildingKind.MonasteryBuilding && Settings.banMonastery)
            {
                SetLastError(Strings.ERROR_BAN_MONASTERY);
                return BuildingBuildError.Ban;
            }

            
            TownModel town = map.GetTownByID(townID);
            if (town == null)
            {
                SetLastError(Strings.ERROR_INVALID_TOWN_ID);
                return BuildingBuildError.InvalidTownID;
            }

            int buildingPos = town.FindBuildingByHexaID(hexaID);
            if (buildingPos == -1)
            {
                SetLastError(Strings.ERROR_INVALID_HEXA_ID);
                return BuildingBuildError.TownHasNoHexaWithThatHexaID;
            }

            HexaModel hexa = town.GetHexa(buildingPos);
            if (hexa.GetKind() == HexaKind.Desert && kind == BuildingKind.SourceBuilding)
            {
                SetLastError(Strings.ERROR_NO_SOURCE_BUILDING_FOR_DESERT);
                return BuildingBuildError.NoSourceBuildingForDesert;
            }
            if (hexa.GetKind() == HexaKind.Water)
            {
                SetLastError(Strings.ERROR_NO_BUILDING_FOR_WATER);
                return BuildingBuildError.NoBuildingForWater;
            }
            if (hexa.GetKind() == HexaKind.Mountains && kind != BuildingKind.SourceBuilding)
            {
                SetLastError(Strings.ERROR_NO_SPECIAL_BUIDLING_FOR_MOUNTAINS);
                return BuildingBuildError.NoSpecialBuildingForMountains;
            }

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
                mapView.AddToViewQueue(item, gm.GetState() == EGameState.BeforeGame);
                if (gm.GetState() != EGameState.BeforeGame)
                {
                    gm.GetActivePlayer().PayForSomething(town.GetBuildingCost(buildingPos, kind));
                }

                town.BuildBuilding(buildingPos, kind);

                TriggerManager.Inst().TurnTrigger(TriggerType.BuildingBuild, (int) kind);
                return true;
            }

            return false;
        }

        public TownBuildError CanBuildTown(int townID)
        {
            TownModel town = map.GetTownByID(townID);
            if (town == null)
            {
                SetLastError(Strings.ERROR_INVALID_TOWN_ID);
                return TownBuildError.InvalidTownID;
            }
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
                PathNode.SetIsValid(false);
                town.BuildTown(gm.GetActivePlayer());

                ItemQueue item = new TownItemQueue(mapView, townID);
                mapView.AddToViewQueue(item);

                if (gm.GetState() == EGameState.BeforeGame)
                    return town;

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
                    gm.GetActivePlayer().PayForSomething(GetPrice(PriceKind.BTown));

                TriggerManager.Inst().TurnTrigger(TriggerType.TownBuild, townID);
                return town;
            }

            return null;
        }

        public bool InventUpgrade(SourceBuildingKind building)
        {
            Player p = gm.GetActivePlayer();
            List<IMonastery> monastery = p.GetMonastery();

            foreach (IMonastery m in monastery)
            {
                switch (m.CanInventUpgrade(building))
                {
                    case MonasteryError.BanSecondUpgrade:
                        return false;
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
            Player p = gm.GetActivePlayer();
            List<IMonastery> monastery = p.GetMonastery();

            foreach (IMonastery m in monastery)
            {
                switch (m.CanInventUpgrade(building))
                {
                    case MonasteryError.HaveSecondUpgrade:
                        SetLastError(Strings.ERROR_HAVE_SECOND_UPGRADE);
                        return MonasteryError.HaveSecondUpgrade;
                    case MonasteryError.NoSources:
                        SetLastError(Strings.ERROR_NO_SOURCES);
                        return MonasteryError.NoSources;
                    case MonasteryError.BanSecondUpgrade:
                        SetLastError(Strings.ERROR_BAN_SECOND_UPGRADE);
                        return MonasteryError.BanSecondUpgrade;
                    case MonasteryError.OK:
                        return MonasteryError.OK;
                    case MonasteryError.MaxUpgrades:          // May be free slot in another monastery
                        break;
                }
            }

            SetLastError(Strings.ERROR_MAX_UPGRADES);
            return MonasteryError.MaxUpgrades;
        }

        public bool BuyLicence(SourceKind source)
        {
            Player p = gm.GetActivePlayer();
            List<IMarket> market = p.GetMarket();

            foreach (IMarket m in market)
            {
                switch (m.CanBuyLicence(source))
                {
                    case MarketError.HaveSecondLicence:
                    case MarketError.BanSecondLicence:
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
            Player p = gm.GetActivePlayer();
            List<IMarket> market = p.GetMarket();

            foreach (IMarket m in market)
            {
                switch (m.CanBuyLicence(source))
                {
                    case MarketError.BanSecondLicence :
                        SetLastError(Strings.ERROR_BAN_SECOND_LICENCE);
                        return MarketError.BanSecondLicence;
                    case MarketError.HaveSecondLicence :
                        SetLastError(Strings.ERROR_HAVE_SECOND_LICENCE);
                        return MarketError.HaveSecondLicence;
                    case MarketError.NoSources :
                        SetLastError(Strings.ERROR_NO_SOURCES);
                        return MarketError.NoSources;
                    case MarketError.OK :
                        return MarketError.OK;
                    case MarketError.MaxLicences :          // May be free slot in another market
                        break;
                }
            }

            SetLastError(Strings.ERROR_MAX_LICENCES);
            return MarketError.MaxLicences;
        }

        public bool CaptureHexa(IHexa hexa)
        {
            List<IFort> forts = GetPlayerMe().GetFort();
            CaptureHexaError error;

            foreach (IFort fort in forts)
            {
                HexaModel.SetHexaFort(fort);
                error = CanCaptureHexa(hexa.GetID(), fort);

                if (error == CaptureHexaError.OK)
                    return CaptureHexa(hexa.GetID(), fort);
            }

            return false;
        }

        public CaptureHexaError CanCaptureHexa(IHexa hexa)
        {
            if (Settings.banFortCaptureHexa)
                return CaptureHexaError.Ban;
            if (hexa == null)
                return CaptureHexaError.InvalidHexaID;

            List<IFort> forts = GetPlayerMe().GetFort();
            CaptureHexaError error;

            foreach (IFort fort in forts)
            {
                HexaModel.SetHexaFort(fort);
                error = CanCaptureHexa(hexa.GetID(), fort);
                if (error == CaptureHexaError.TooFarFromFort)
                    continue;

                return error;
            }

            return CaptureHexaError.TooFarFromFort;
        }

        public CaptureHexaError CanCaptureHexa(int hexaID, IFort fort)
        {
            HexaModel hexa = map.GetHexaByID(hexaID);
            HexaModel.SetHexaFort(fort);
            if (!hexa.IsInFortRadius())
            {
                SetLastError(Strings.ERROR_TOO_FAR_FROM_FORT);
                return CaptureHexaError.TooFarFromFort;
            }

            if (Settings.banFortCaptureHexa)
            {
                SetLastError(Strings.ERROR_BAN_FORT_CAPTURE_HEXA);
                return CaptureHexaError.Ban;
            }

            if (!Settings.costFortCapture.HasPlayerSources(gm.GetActivePlayer()))
            {
                SetLastError(Strings.ERROR_NO_SOURCES);
                return CaptureHexaError.NoSources;
            }

            if (hexa.GetKind() == HexaKind.Water || hexa.GetKind() == HexaKind.Null)
                return CaptureHexaError.Desert;

            if (hexa == null)
            {
                SetLastError(Strings.ERROR_INVALID_HEXA_ID);
                return CaptureHexaError.InvalidHexaID;
            }

            return CaptureHexaError.OK;
        }

        public bool CaptureHexa(int hexaID, IFort fort)
        {
            if (CanCaptureHexa(hexaID, fort) == CaptureHexaError.OK)
            {
                HexaModel hexa = map.GetHexaByID(hexaID);
                Player player = gm.GetActivePlayer();
                hexa.Capture(player);
                player.PayForSomething(Settings.costFortCapture);
                player.AddFortAction();
                player.AddPoints(PlayerPoints.FortCaptureHexa);
                return true;
            }
            return false;
        }


        public bool IsInFortRadius(IHexa hexa, IPlayer player)
        {
            List<IFort> forts = player.GetFort();

            foreach (IFort fort in forts)
            {
                HexaModel.SetHexaFort(fort);
                HexaModel model = (HexaModel)hexa;
                if (model.IsInFortRadius())
                    return true;
            }
            return false;
        }

        public DestroyHexaError CanDestroyHexa(int hexaID, IFort fort)
        {
            if (!Settings.costFortCrusade.HasPlayerSources(gm.GetActivePlayer()))
            {
                SetLastError(Strings.ERROR_NO_SOURCES);
                return DestroyHexaError.NoSources;
            }

            if (fort == null)
                return DestroyHexaError.OK;

            HexaModel.SetHexaFort(fort);

            HexaModel hexa = map.GetHexaByID(hexaID);

            if (hexa == null)
            {
                SetLastError(Strings.ERROR_INVALID_HEXA_ID);
                return DestroyHexaError.InvalidHexaID;
            }

            if (!hexa.IsInFortRadius())
            {
                SetLastError(Strings.ERROR_TOO_FAR_FROM_FORT);
                return DestroyHexaError.TooFarFromFort;
            }
            /*
            if (hexa.GetDestroyed())
            {
                SetLastError(Strings.ERROR_INVALID_HEXA_ID);
                return DestroyHexaError.IsDestroyed;
            }*/

            return DestroyHexaError.OK;
        }

        public bool DestroyHexa(int hexaID, IFort fort)
        {
            if (CanDestroyHexa(hexaID, fort) == DestroyHexaError.OK)
            {
                HexaModel hexa = map.GetHexaByID(hexaID);
               
                SourceAll source = new SourceAll(0);
                int amount = gm.GetRandomInt(70) + ((hexa.GetDestroyed()) ? 0 : 70);
                hexa.Destroy();
                
                switch(hexa.GetKind())
                {
                    case HexaKind.Cornfield :
                        source = new SourceAll(amount, 0, 0, 0, 0);
                        break;
                    case HexaKind.Pasture :
                        source = new SourceAll(0, amount, 0, 0, 0);
                        break;
                    case HexaKind.Stone :
                        source = new SourceAll(0, 0, amount, 0, 0);
                        break;
                    case HexaKind.Forest :
                        source = new SourceAll(0, 0, 0, amount, 0);
                        break;
                    case HexaKind.Mountains :
                        source = new SourceAll(0, 0, 0, 0, amount);
                        break;
                }
                gm.GetActivePlayer().PayForSomething(Settings.costFortCrusade - source);
                gm.GetActivePlayer().AddFortAction();
                return true;
            }
            return false;
        }

        public DestroySourcesError CanStealSources(String playerName)
        {
            if (Settings.banFortStealSources)
            {
                SetLastError(Strings.ERROR_BAN_FORT_STEAL_SOURCES);
                return DestroySourcesError.Ban;
            }

            Player player = gm.GetPlayer(playerName);
            if (player == null)
                return DestroySourcesError.NoPlayerWithName;

            Player activePlayer = gm.GetActivePlayer();
            if (activePlayer.GetBuildingCount(Building.Fort) == 0)
            {
                SetLastError(Strings.ERROR_NO_FORT);
                return DestroySourcesError.NoFort;
            }
            if (!Settings.costFortSources.HasPlayerSources(activePlayer))
            {
                SetLastError(Strings.ERROR_NO_SOURCES);
                return DestroySourcesError.NoSources;
            }

            return DestroySourcesError.OK;
        }

        public bool StealSources(String playerName)
        {
            Player player = gm.GetPlayer(playerName);
            Player activePlayer = gm.GetActivePlayer();

            if (CanStealSources(playerName) == DestroySourcesError.OK)
            {
                SourceAll source = (SourceAll) player.GetSource();
                player.PayForSomething(new SourceAll(source / 2));
                activePlayer.AddSources(-Settings.costFortSources, TransactionState.TransactionStart);
                activePlayer.AddSources(source / 2, TransactionState.TransactionEnd);
                activePlayer.AddFortAction();
                activePlayer.AddPoints(PlayerPoints.FortStealSources);
                if (activePlayer.GetIsAI())
                {
                    Message.Inst().Show("Někdo krade!", activePlayer.GetName() + " se vloupal do skladišť " + player.GetName() + " a odnesl si lup polovinu jeho zásob.", GameResources.Inst().GetHudTexture(HUDTexture.IconFortSources));
                }
                return true;
            }
            return false;
        }

        public ParadeError CanShowParade()
        {
            if (Settings.banFortParade)
            {
                SetLastError(Strings.ERROR_BAN_FORT_SHOW_PARADE);
                return ParadeError.Ban;
            }

            Player activePlayer = gm.GetActivePlayer();

            if (!GetPrice(PriceKind.AParade).HasPlayerSources(activePlayer))
            {
                SetLastError(Strings.ERROR_NO_SOURCES);
                return ParadeError.NoSources;
            }
            if (activePlayer.GetBuildingCount(Building.Fort) == 0)
            {
                SetLastError(Strings.ERROR_NO_FORT);
                return ParadeError.NoFort;
            }

            return ParadeError.OK;
        }

        public bool ShowParade()
        {
            if (CanShowParade() == ParadeError.OK)
            {
                Player activePlayer = gm.GetActivePlayer();
                activePlayer.AddPoints(PlayerPoints.FortParade);
                activePlayer.AddFortAction();
                activePlayer.PayForSomething(GetPrice(PriceKind.AParade));
                if (gm.GetActivePlayer().GetIsAI())
                {
                    Message.Inst().Show(Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_PARADE, Strings.PROMPT_DESCTIPTION_MESSAGE_FORT_ACTION_PARADE, GameResources.Inst().GetHudTexture(HUDTexture.IconFortParade));
                }
                return true;
            }

            return false;
        }

        public bool ChangeSourcesFor(List<ISourceAll> sourceList)
        {
            SourceAll source = new SourceAll(0);

            foreach (ISourceAll isource in sourceList)
            {
                source = source + (SourceAll)isource;
            }

            return ChangeSourcesFor(source);
        }

        public bool ChangeSourcesFor(ISourceAll source)
        {
            if (CanChangeSourcesFor(source) < 0)
            {
                SetLastError(Strings.ERROR_NOT_ENOUGHT_FROM_SOURCE);
                return false;
            }

            Player player = gm.GetActivePlayer();
            if (source.HasPlayerSources(player))
                return true;

            SourceAll source2 = (SourceAll)source;
            SourceAll delta = (SourceAll) player.GetSource() - source2;
            SourceAll collectSources = (SourceAll) player.GetCollectSourcesNormal();
            SourceKind[] kindOrdered = collectSources.Order();

            int plusSources = 0;

            for (int plus = 4; plus >= 0; plus--)
            {
                if (delta[(int) kindOrdered[plus]] > 0)
                {
                    for (int minus = 0; minus < 5; minus++)
                    {
                        if (delta[minus] < 0)
                        {
                            plusSources = delta[(int)kindOrdered[plus]] / player.GetConversionRate(kindOrdered[plus]);

                            if (plusSources > -delta[minus])
                            {
                                if (ChangingSourcesError.OK == ChangeSources(kindOrdered[plus], (SourceKind)minus, -delta[minus] * player.GetConversionRate(kindOrdered[plus])))
                                {
                                    delta[(int)kindOrdered[plus]] += delta[minus] * player.GetConversionRate(kindOrdered[plus]);
                                    delta[minus] = 0;
                                }
                                else
                                {
                                    throw new Exception("Clever changing sources is not as clever as developers thought.");
                                }
                            }
                            else
                            {
                                if (ChangingSourcesError.OK == ChangeSources(kindOrdered[plus], (SourceKind)minus, plusSources * player.GetConversionRate(kindOrdered[plus])))
                                {
                                    delta[(int)kindOrdered[plus]] -= plusSources * player.GetConversionRate(kindOrdered[plus]);
                                    delta[minus] += plusSources;
                                }
                                else
                                {
                                    throw new Exception("Clever changing sources is not as clever as developers thought.");
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public int CanChangeSourcesFor(ISourceAll source)
        {
            SourceAll source2 = (SourceAll) source;

            Player player = gm.GetActivePlayer();

            //if (source.HasPlayerSources(player))
            //    return 0;

            SourceAll delta = (SourceAll) player.GetSource() - source2;

            int minusSources = 0;
            int plusSources = 0;
            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (delta[loop1] < 0)
                    minusSources += -delta[loop1];
                else
                    plusSources += delta[loop1] / player.GetConversionRate((SourceKind)loop1);
            }

            if (minusSources > plusSources)
                return -1;
            return minusSources;
        }

        private void FindWaysToAllTowns(IPlayer player)
        {
            if (PathNode.GetIsValid() && PathNode.GetPlayerReference() == player)
                return;

            PathNode.SetIsValid(false);
            for (int loop1 = 1; loop1 <= GetMaxTownID(); loop1++)
            {
                TownModel town = (TownModel)GetITownByID(loop1);
               
                town.ClearNodePath();
            }

            for (int loop1 = 1; loop1 <= GetMaxRoadID(); loop1++)
            {
                RoadModel road = (RoadModel) GetIRoadByID(loop1);
                road.ClearNodePath();
            }

            Queue<TownModel> openList = new Queue<TownModel>();
            List<ITown> buildedTowns = player.GetTown();

            foreach (ITown itown in buildedTowns)
            {
                TownModel town = (TownModel)itown;
                town.SetPathNode(0, null, null);
                openList.Enqueue(town);
            }
            List<IRoad> buildedRoads = player.GetRoad();
            foreach (IRoad road in buildedRoads)
            {
                TownModel town;
                RoadModel roadM = (RoadModel) road;
                roadM.SetPathNode(0, null, null);

                foreach(ITown itown in road.GetITown())
                {
                    town = (TownModel)itown;
                    if (town.GetPathNode().GetDistance() == PathNode.INFINITY &&
                        (!town.GetIsBuild() || town.GetIOwner() == player))
                    {
                        town.SetPathNode(0, null, null);
                        openList.Enqueue(town);
                    }
                }
            }

            while (openList.Count > 0)
            {

                TownModel ancestor = openList.Dequeue();
                int dst = ancestor.GetPathNode().GetDistance();

                for(byte loop1 = 0; loop1 < 3; loop1++)
                {
                    IRoad road = ancestor.GetIRoad(loop1);
                    if (road != null &&
                        !road.GetIsBuild())
                    {
                        RoadModel roadM = (RoadModel)road;
                        roadM.SetPathNode(dst + 1, ancestor, ancestor.GetPathNode().GetAncestorRoad());
                        foreach (ITown itown in road.GetITown())
                        {
                            if (itown != null && 
                                itown.GetTownID() != ancestor.GetTownID() &&
                                itown.GetIOwner() == null)
                            {
                                TownModel town = (TownModel)itown;
                                if (!openList.Contains(town) && town.GetPathNode().GetDistance() == PathNode.INFINITY)
                                {
                                    town.SetPathNode(dst + 1, ancestor, road);
                                    openList.Enqueue(town);
                                }
                            }
                        }
                    }
                }
            }

            PathNode.SetIsValid(true);
            PathNode.SetPlayerReference(player);
        }

        public int GetDistanceToRoad(IRoad road, IPlayer player)
        {
            FindWaysToAllTowns(player);

            RoadModel roadModel = (RoadModel)road;
            return roadModel.GetPathNode().GetDistance();
        }

        public int GetDistanceToTown(ITown town, IPlayer player)
        {
            FindWaysToAllTowns(player);

            for (byte loop1 = 0; loop1 <= 2; loop1++)
            {
                if (town.GetIHexa(loop1).GetCapturedIPlayer() == player)
                    return 0;
            }

            TownModel townModel = (TownModel)town;
            return townModel.GetPathNode().GetDistance();
        }

        public List<IRoad> GetRoadsToTown(ITown town, IPlayer player)
        {
            FindWaysToAllTowns(player);

            List<IRoad> roadWay = new List<IRoad>();
            for (byte loop1 = 0; loop1 <= 2; loop1++)
            {
                if (town.GetIHexa(loop1).GetCapturedIPlayer() == player)
                    return roadWay;
            }


            TownModel townModel = (TownModel)town;

            while (townModel.GetPathNode().GetAncestorTown() != null)
            {        
                PathNode node = townModel.GetPathNode();

                roadWay.Add(node.GetAncestorRoad());
                townModel = node.GetAncestorTown();
            }

            roadWay.Reverse();

            return roadWay;
        }

        public int GetDistance(ITown a, ITown b)
        {
            if (a == null || b == null)
                return -1;

            if (a.GetTownID() == b.GetTownID())
                return 0;

            Queue<ITown> open = new Queue<ITown>();
            Queue<int> distances = new Queue<int>();
            Queue<ITown> close = new Queue<ITown>();

            distances.Enqueue(0);
            open.Enqueue(a);

            ITown tempTown1, tempTown2;
            int tempDistance;
            while (open.Count > 0)
            {
                tempTown1 = open.Dequeue();
                tempDistance = distances.Dequeue();

                if (tempTown1.GetTownID() == b.GetTownID())
                {
                    return tempDistance;
                }

                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    tempTown2 = tempTown1.GetITown(loop1);
                    if (tempTown2 == null)
                        continue;

                    bool isFresh = true;

                    foreach (ITown closeTown in close)
                    {
                        if (tempTown2.GetTownID() == closeTown.GetTownID())
                        {
                            isFresh = false;
                            break;
                        }
                    }

                    if(isFresh)
                        foreach (ITown openTown in open)
                        {
                            if (tempTown2.GetTownID() == openTown.GetTownID())
                            {
                                isFresh = false;
                                break;
                            }
                        }

                    if (isFresh)
                    {
                        open.Enqueue(tempTown2);
                        distances.Enqueue(tempDistance + 1);
                    }
                }

                close.Enqueue(tempTown1);
            }


            // Towns a and b are on different islands
            return Int16.MaxValue; 
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
                case PriceKind.MCorn1: return Settings.costMarketCorn1;
                case PriceKind.MMeat1: return Settings.costMarketMeat1;
                case PriceKind.MStone1: return Settings.costMarketStone1;
                case PriceKind.MWood1: return Settings.costMarketWood1;
                case PriceKind.MOre1: return Settings.costMarketOre1;
                case PriceKind.MCorn2: return Settings.costMarketCorn2;
                case PriceKind.MMeat2: return Settings.costMarketMeat2;
                case PriceKind.MStone2: return Settings.costMarketStone2;
                case PriceKind.MWood2: return Settings.costMarketWood2;
                case PriceKind.MOre2: return Settings.costMarketOre2;
                case PriceKind.AParade: return Settings.costFortParade;
                case PriceKind.ACaptureHexa: return Settings.costFortCapture;
                case PriceKind.AStealSources: return Settings.costFortSources;
            }

            return null;
        }

        public int GetPoints(PriceKind kind)
        {
            switch (kind)
            {
                case PriceKind.BRoad: return Settings.pointsRoad;
                case PriceKind.BTown: return Settings.pointsTown;
                case PriceKind.BFort: return Settings.pointsFort;
                case PriceKind.BMarket: return Settings.pointsMarket;
                case PriceKind.BMonastery: return Settings.pointsMonastery;
                case PriceKind.BMill: return Settings.pointsMill;
                case PriceKind.BStepherd: return Settings.pointsStepherd;
                case PriceKind.BSaw: return Settings.pointsSaw;
                case PriceKind.BQuarry: return Settings.pointsQuarry;
                case PriceKind.BMine: return Settings.pointsMine;
                case PriceKind.UMill1: return Settings.pointsUpgradeLvl1;
                case PriceKind.UStepherd1: return Settings.pointsUpgradeLvl1;
                case PriceKind.UQuarry1: return Settings.pointsUpgradeLvl1;
                case PriceKind.USaw1: return Settings.pointsUpgradeLvl1;
                case PriceKind.UMine1: return Settings.pointsUpgradeLvl1;
                case PriceKind.UMill2: return Settings.pointsUpgradeLvl2;
                case PriceKind.UStepherd2: return Settings.pointsUpgradeLvl2;
                case PriceKind.UQuarry2: return Settings.pointsUpgradeLvl2;
                case PriceKind.USaw2: return Settings.pointsUpgradeLvl2;
                case PriceKind.UMine2: return Settings.pointsUpgradeLvl2;
                case PriceKind.MCorn1: return Settings.pointsMarketLvl1;
                case PriceKind.MMeat1: return Settings.pointsMarketLvl1;
                case PriceKind.MStone1: return Settings.pointsMarketLvl1;
                case PriceKind.MWood1: return Settings.pointsMarketLvl1;
                case PriceKind.MOre1: return Settings.pointsMarketLvl1;
                case PriceKind.MCorn2: return Settings.pointsMarketLvl2;
                case PriceKind.MMeat2: return Settings.pointsMarketLvl2;
                case PriceKind.MStone2: return Settings.pointsMarketLvl2;
                case PriceKind.MWood2: return Settings.pointsMarketLvl2;
                case PriceKind.MOre2: return Settings.pointsMarketLvl2;
                case PriceKind.AParade: return Settings.pointsFortParade;
                case PriceKind.ACaptureHexa: return Settings.pointsFortCapture;
                case PriceKind.AStealSources: return Settings.pointsFortSteal;
            }

            return 0;
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

        public void Log(string srcFile, string msg)
        {
            Logger.Inst().Log(srcFile + gm.GetActivePlayer().GetName() + ".txt", msg);
        }

        private PriceKind GetPriceForMonasteryUpgrade(UpgradeKind upgradeKind, SourceBuildingKind buildingKind)
        {
            switch (upgradeKind)
            {
                case UpgradeKind.FirstUpgrade:
                    switch (buildingKind)
                    {
                        case SourceBuildingKind.Mill: return PriceKind.UMill1;
                        case SourceBuildingKind.Stepherd: return PriceKind.UStepherd1;
                        case SourceBuildingKind.Quarry: return PriceKind.UQuarry1;
                        case SourceBuildingKind.Saw: return PriceKind.USaw1;
                        case SourceBuildingKind.Mine: return PriceKind.UMine1;
                    }
                    break;
                case UpgradeKind.SecondUpgrade:
                    switch (buildingKind)
                    {
                        case SourceBuildingKind.Mill: return PriceKind.UMill2;
                        case SourceBuildingKind.Stepherd: return PriceKind.UStepherd2;
                        case SourceBuildingKind.Quarry: return PriceKind.UQuarry2;
                        case SourceBuildingKind.Saw: return PriceKind.USaw2;
                        case SourceBuildingKind.Mine: return PriceKind.UMine2;
                    }
                    break;
            }
            return PriceKind.UMill1;
        }

        private PriceKind GetPriceForMarketLicence(LicenceKind licenceKind, SourceKind sourceKind)
        {
            switch (licenceKind)
            {
                case LicenceKind.FirstLicence:
                    switch (sourceKind)
                    {
                        case SourceKind.Corn: return PriceKind.MCorn1;
                        case SourceKind.Meat: return PriceKind.MMeat1;
                        case SourceKind.Stone: return PriceKind.MStone1;
                        case SourceKind.Wood: return PriceKind.MWood1;
                        case SourceKind.Ore: return PriceKind.MOre1;
                    }
                    break;

                case LicenceKind.SecondLicence:
                    switch (sourceKind)
                    {
                        case SourceKind.Corn: return PriceKind.MCorn2;
                        case SourceKind.Meat: return PriceKind.MMeat2;
                        case SourceKind.Stone: return PriceKind.MStone2;
                        case SourceKind.Wood: return PriceKind.MWood2;
                        case SourceKind.Ore: return PriceKind.MOre2;
                    }
                    break;
            }
            return PriceKind.MCorn1;
        }

        #region IMapController Members


        public int GetTurnNumber()
        {
            return gm.GetTurnNumber();
        }

        public IGameSetting GetGameSettings()
        {
            return gm.GetGameSettings();
        }

        public ISourceAll GetPrice(SourceKind sourceKind, LicenceKind licenceKind)
        {
            if (licenceKind == LicenceKind.NoLicence)
                return new SourceAll(0);

            return GetPrice(GetPriceForMarketLicence(licenceKind, sourceKind));
        }

        public ISourceAll GetPrice(SourceBuildingKind buildingKind, UpgradeKind upgradeKind)
        {
            if (upgradeKind == UpgradeKind.NoUpgrade)
                return new SourceAll(0);

            return GetPrice(GetPriceForMonasteryUpgrade(upgradeKind, buildingKind));
        }

        public bool IsBanAction(PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.BuildFort: return Settings.banFort;
                case PlayerAction.BuildMarket: return Settings.banMarket;
                case PlayerAction.BuildMonastery: return Settings.banMonastery;
                case PlayerAction.BuySecondLicence: return Settings.banSecondLicence;
                case PlayerAction.InventSecondUpgrade: return Settings.banSecondUpgrade;
                case PlayerAction.FortCaptureHexa: return Settings.banFortCaptureHexa;
                case PlayerAction.FortStealSources: return Settings.banFortStealSources;
                case PlayerAction.FortParade: return Settings.banFortParade;
            }
            return false;
        }
        public int GetActionPoints(PlayerPoints action)
        {
            switch (action)
            {
                case PlayerPoints.Fort: return Settings.pointsFort;
                case PlayerPoints.FortCaptureHexa: return Settings.pointsFortCapture;
                case PlayerPoints.FortParade: return Settings.pointsFortParade;
                case PlayerPoints.FortStealSources: return Settings.pointsFortSteal;
                case PlayerPoints.LicenceLvl1: return Settings.pointsMarketLvl1;
                case PlayerPoints.LicenceLvl2: return Settings.pointsMarketLvl2;
                case PlayerPoints.Market: return Settings.pointsMarket;
                case PlayerPoints.RoadID: return Settings.goalRoadID;
                case PlayerPoints.TownID: return Settings.goalTownID;
                case PlayerPoints.Mill: return Settings.pointsMill;
                case PlayerPoints.Mine: return Settings.pointsMine;
                case PlayerPoints.Monastery: return Settings.pointsMonastery;
                case PlayerPoints.Quarry: return Settings.pointsQuarry;
                case PlayerPoints.Road: return Settings.pointsRoad;
                case PlayerPoints.Saw: return Settings.pointsSaw;
                case PlayerPoints.Stepherd: return Settings.pointsStepherd;
                case PlayerPoints.Town: return Settings.pointsTown;
                case PlayerPoints.UpgradeLvl1: return Settings.pointsUpgradeLvl1;
                case PlayerPoints.UpgradeLvl2: return Settings.pointsUpgradeLvl2;
            }
            return 0;
        }

        #endregion
    }
}
