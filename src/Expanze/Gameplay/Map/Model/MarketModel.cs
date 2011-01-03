using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;

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
            GameMaster.Inst().GetActivePlayer().BuyMarketLicence(licenceKind, upgradeNumber);
        }

        public override void SetPromptWindow(PromptWindow.Mod mod)
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.Show(mod, Strings.PROMPT_TITLE_WANT_TO_BUILD_MARKET, true);
            if(owner.GetMarketLicence(SourceKind.Corn) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_1, Settings.costMarketCorn1, true, res.GetHudTexture(HUDTexture.IconCorn1)));
            else if (owner.GetMarketLicence(SourceKind.Corn) == LicenceKind.FirstLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_2, Settings.costMarketCorn2, true, res.GetHudTexture(HUDTexture.IconCorn2)));

            if (owner.GetMarketLicence(SourceKind.Meat) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1, Settings.costMarketMeat1, true, res.GetHudTexture(HUDTexture.IconMeat1)));
            else if (owner.GetMarketLicence(SourceKind.Meat) == LicenceKind.FirstLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 1, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2, Settings.costMarketMeat2, true, res.GetHudTexture(HUDTexture.IconMeat2)));

            if (owner.GetMarketLicence(SourceKind.Stone) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_1, Settings.costMarketStone1, true, res.GetHudTexture(HUDTexture.IconStone1)));
            else if (owner.GetMarketLicence(SourceKind.Stone) == LicenceKind.FirstLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 2, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_2, Settings.costMarketStone2, true, res.GetHudTexture(HUDTexture.IconStone2)));

            if (owner.GetMarketLicence(SourceKind.Wood) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1, Settings.costMarketWood1, true, res.GetHudTexture(HUDTexture.IconWood1)));
            else if (owner.GetMarketLicence(SourceKind.Wood) == LicenceKind.FirstLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 3, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2, Settings.costMarketWood2, true, res.GetHudTexture(HUDTexture.IconWood2)));

            if (owner.GetMarketLicence(SourceKind.Ore) == LicenceKind.NoLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 4, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_1, Settings.costMarketOre1, true, res.GetHudTexture(HUDTexture.IconOre1)));
            else if (owner.GetMarketLicence(SourceKind.Ore) == LicenceKind.FirstLicence)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 4, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_2, Settings.costMarketOre2, true, res.GetHudTexture(HUDTexture.IconOre2)));
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

        public static BuildingPromptItem getPromptItemBuildMarket(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MarketBuilding, Strings.PROMPT_TITLE_WANT_TO_BUILD_MARKET, Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_MARKET, Settings.costMarket, true, GameResources.Inst().GetHudTexture(HUDTexture.IconMarket));
        }

        public bool BuyLicence(SourceKind source)
        {
            LicenceKind licenceKind = owner.GetMarketLicence(source);
            UpgradeKind upgradeKind;
            switch (licenceKind)
            {
                case LicenceKind.NoLicence: upgradeKind = UpgradeKind.NoUpgrade; break;
                case LicenceKind.FirstLicence: upgradeKind = UpgradeKind.FirstUpgrade; break;
                default: upgradeKind = UpgradeKind.SecondUpgrade; break;
            }

            return GameState.map.GetMapController().BuyUpgradeInSpecialBuilding(townID, hexaID, upgradeKind, (int)source) == BuyingUpgradeError.OK;
        }
    }
}
