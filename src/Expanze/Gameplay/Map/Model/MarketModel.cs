using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Utils.Music;

namespace Expanze.Gameplay
{
    class MarketModel : SpecialBuilding, IMarket
    {
        int townID; // where is this building
        int hexaID;

        public MarketModel(Player playerOwner, int townID, int hexaID) : base(playerOwner)
        {
            this.townID = townID;
            this.hexaID = hexaID;
        }

        public override Texture2D GetIconActive()
        {
            return GameResources.Inst().GetHudTexture(HUDTexture.IconMarketActive);
        }

        public override Texture2D GetIconPassive()
        {
            return GameResources.Inst().GetHudTexture(HUDTexture.IconMarket);
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {
            LicenceKind licenceKind;
            switch (upgradeKind)
            {
                case UpgradeKind.NoUpgrade: licenceKind = LicenceKind.NoLicence; break;
                case UpgradeKind.FirstUpgrade: licenceKind = LicenceKind.FirstLicence; break;
                default: licenceKind = LicenceKind.SecondLicence; break;
            }
            Player p = GameMaster.Inst().GetActivePlayer();
            p.BuyMarketLicence(licenceKind, upgradeNumber);

            if (!p.GetIsAI())
            {
                int tempItem = PromptWindow.Inst().GetActiveItem();
                SetPromptWindow(PromptWindow.Mod.Buyer, true);
                PromptWindow.Inst().SetActiveItem(tempItem);
            }
            else
            {
                Message.Inst().Show(p.GetName() + Strings.Inst().GetString(TextEnum.GAME_ALERT_TITLE_AI_BOUGHT_LICENCE), p.GetName() + Strings.Inst().GetString(TextEnum.GAME_ALERT_DESTRIPTION_AI_BOUGHT_LICENCE1) + GetLicenceKindString(licenceKind) + Strings.Inst().GetString(TextEnum.GAME_ALERT_DESTRIPTION_AI_BOUGHT_LICENCE2) + GetLicenceSourceName(upgradeNumber) + ".", GetLicenceIcon(licenceKind, upgradeNumber));
            }
        }

        public String GetLicenceSourceName(int number)
        {
            switch (number)
            {
                case 0: return Strings.Inst().GetString(TextEnum.GAME_ALERT_AI_BOUGHT_LICENCE_CORN);
                case 1: return Strings.Inst().GetString(TextEnum.GAME_ALERT_AI_BOUGHT_LICENCE_MEAT);
                case 2: return Strings.Inst().GetString(TextEnum.GAME_ALERT_AI_BOUGHT_LICENCE_STONE);
                case 3: return Strings.Inst().GetString(TextEnum.GAME_ALERT_AI_BOUGHT_LICENCE_WOOD);
                case 4: return Strings.Inst().GetString(TextEnum.GAME_ALERT_AI_BOUGHT_LICENCE_ORE);
            }
            return "";
        }

        public String GetLicenceKindString(LicenceKind kind)
        {
            switch (kind)
            {
                case LicenceKind.FirstLicence: return Strings.Inst().GetString(TextEnum.GAME_ALERT_AI_BOUGHT_LICENCE_1);
                case LicenceKind.SecondLicence: return Strings.Inst().GetString(TextEnum.GAME_ALERT_AI_BOUGHT_LICENCE_2);
            }
            return "";
        }

        public Texture2D GetLicenceIcon(LicenceKind kind, int licenceNumber)
        {
            switch (kind)
            {
                case LicenceKind.FirstLicence :
                    switch (licenceNumber)
                    {
                        case 0: return GameResources.Inst().GetHudTexture(HUDTexture.IconCorn1);
                        case 1: return GameResources.Inst().GetHudTexture(HUDTexture.IconMeat1);
                        case 2: return GameResources.Inst().GetHudTexture(HUDTexture.IconStone1);
                        case 3: return GameResources.Inst().GetHudTexture(HUDTexture.IconWood1);
                        case 4: return GameResources.Inst().GetHudTexture(HUDTexture.IconOre1);
                    }
                    break;
                case LicenceKind.SecondLicence:
                    switch (licenceNumber)
                    {
                        case 0: return GameResources.Inst().GetHudTexture(HUDTexture.IconCorn2);
                        case 1: return GameResources.Inst().GetHudTexture(HUDTexture.IconMeat2);
                        case 2: return GameResources.Inst().GetHudTexture(HUDTexture.IconStone2);
                        case 3: return GameResources.Inst().GetHudTexture(HUDTexture.IconWood2);
                        case 4: return GameResources.Inst().GetHudTexture(HUDTexture.IconOre2);
                    }
                    break;
            }

            return null;
        }

        public override void SetPromptWindow(PromptWindow.Mod mod, bool silent)
        {
            if(!silent)
                MusicManager.Inst().PlaySound(SoundEnum.market);
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.Show(mod, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUILD_MARKET), true);
            if(owner.GetMarketLicence(SourceKind.Corn) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_1), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_1), Settings.costMarketCorn1, true, res.GetHudTexture(HUDTexture.IconCorn1)));
            else if (owner.GetMarketLicence(SourceKind.Corn) == LicenceKind.FirstLicence && !Settings.banSecondLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_2), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_2), Settings.costMarketCorn2, true, res.GetHudTexture(HUDTexture.IconCorn2)));

            if (owner.GetMarketLicence(SourceKind.Meat) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1), Settings.costMarketMeat1, true, res.GetHudTexture(HUDTexture.IconMeat1)));
            else if (owner.GetMarketLicence(SourceKind.Meat) == LicenceKind.FirstLicence && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 1, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2), Settings.costMarketMeat2, true, res.GetHudTexture(HUDTexture.IconMeat2)));

            if (owner.GetMarketLicence(SourceKind.Stone) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_1), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_1), Settings.costMarketStone1, true, res.GetHudTexture(HUDTexture.IconStone1)));
            else if (owner.GetMarketLicence(SourceKind.Stone) == LicenceKind.FirstLicence && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 2, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_2), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_2), Settings.costMarketStone2, true, res.GetHudTexture(HUDTexture.IconStone2)));

            if (owner.GetMarketLicence(SourceKind.Wood) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1), Settings.costMarketWood1, true, res.GetHudTexture(HUDTexture.IconWood1)));
            else if (owner.GetMarketLicence(SourceKind.Wood) == LicenceKind.FirstLicence && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 3, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2), Settings.costMarketWood2, true, res.GetHudTexture(HUDTexture.IconWood2)));

            if (owner.GetMarketLicence(SourceKind.Ore) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 4, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_1), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_1), Settings.costMarketOre1, true, res.GetHudTexture(HUDTexture.IconOre1)));
            else if (owner.GetMarketLicence(SourceKind.Ore) == LicenceKind.FirstLicence && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 4, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_2), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_2), Settings.costMarketOre2, true, res.GetHudTexture(HUDTexture.IconOre2)));

            if (win.GetItemCount() == 0)
            {
                win.AddPromptItem(new PromptItem(Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUILD_MARKET), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_ALL_LICENCES_BOUGHT), new SourceAll(0), false, false, GameResources.Inst().GetHudTexture(HUDTexture.IconMarket)));
            }
        }

        public override bool GetFreePlaceForUpgrade()
        {
            return owner.LicenceFreeSlot > 0;
        }

        public override SourceAll GetUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber)
        {
            switch (upgradeKind)
            {
                case UpgradeKind.FirstUpgrade :
                    switch (upgradeNumber)
                    {
                        case 0: return Settings.costMarketCorn1;
                        case 1: return Settings.costMarketMeat1;
                        case 2: return Settings.costMarketStone1;
                        case 3: return Settings.costMarketWood1;
                        case 4: return Settings.costMarketOre1;
                    }
                    break;
                case UpgradeKind.SecondUpgrade :
                    switch (upgradeNumber)
                    {
                        case 0: return Settings.costMarketCorn2;
                        case 1: return Settings.costMarketMeat2;
                        case 2: return Settings.costMarketStone2;
                        case 3: return Settings.costMarketWood2;
                        case 4: return Settings.costMarketOre2;
                    }
                    break;
            }

            return new SourceAll(0);
        }

        public static BuildingPromptItem GetPromptItemBuildMarket(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MarketBuilding, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUILD_MARKET), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUILD_MARKET), Settings.costMarket, true, GameResources.Inst().GetHudTexture(HUDTexture.IconMarket));
        }

        public bool BuyLicence(SourceKind source)
        {
            LicenceKind licenceKind = owner.GetMarketLicence(source);
            UpgradeKind upgradeKind;
            switch (licenceKind)
            {
                case LicenceKind.NoLicence: upgradeKind = UpgradeKind.FirstUpgrade; break;
                case LicenceKind.FirstLicence: upgradeKind = UpgradeKind.SecondUpgrade; break;
                default: return false;
            }

            return GameState.map.GetMapController().BuyUpgradeInSpecialBuilding(townID, hexaID, upgradeKind, (int)source);
        }

        public MarketError CanBuyLicence(SourceKind source)
        {
            LicenceKind licenceKind = owner.GetMarketLicence(source);
            UpgradeKind upgradeKind;
            switch (licenceKind)
            {
                case LicenceKind.NoLicence: upgradeKind = UpgradeKind.FirstUpgrade; break;
                case LicenceKind.FirstLicence: upgradeKind = UpgradeKind.SecondUpgrade; break;
                default: upgradeKind = UpgradeKind.NoUpgrade; break;
            }

            if (upgradeKind == UpgradeKind.SecondUpgrade &&
               Settings.banSecondLicence)
                return MarketError.BanSecondLicence;

            BuyingUpgradeError error = GameState.map.GetMapController().CanBuyUpgradeInSpecialBuilding(townID, hexaID, upgradeKind, (int)source);

            switch (error)
            {
                case BuyingUpgradeError.YouAlreadyHaveSecondUpgrade:
                    GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.ERROR_HAVE_SECOND_LICENCE));
                    return MarketError.HaveSecondLicence;
                case BuyingUpgradeError.NoSources:
                    GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.ERROR_NO_SOURCES));
                    return MarketError.NoSources;
                case BuyingUpgradeError.MaxUpgrades:
                    GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.ERROR_MAX_LICENCES));
                    return MarketError.MaxLicences;
                case BuyingUpgradeError.OK: return MarketError.OK;
            }

            return MarketError.OK;
        }

        #region IMarket Members


        public int GetFreeSlot()
        {
            return upgradeMax - upgradeCount;
        }

        #endregion
    }
}
