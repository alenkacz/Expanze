using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay
{
    class MarketModel : SpecialBuilding
    {
        int townID; // where is this building
        int hexaID;

        public MarketModel(int townID, int hexaID)
        {
            this.townID = townID;
            this.hexaID = hexaID;
        }

        public override Texture2D GetIconActive()
        {
            return GameResources.Inst().getHudTexture(HUDTexture.IconMarketActive);
        }

        public override Texture2D GetIconPassive()
        {
            return GameResources.Inst().getHudTexture(HUDTexture.IconMarket);
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster.Inst().GetActivePlayer().SetMarketRate(upgradeKind, upgradeNumber);
        }

        public override void setPromptWindow(PromptWindow.Mod mod)
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.Show(mod, Strings.PROMPT_TITLE_WANT_TO_BUILD_MARKET, true);
            if(upgradeFirst[0] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_1, Settings.costMarketCorn1, true, res.getHudTexture(HUDTexture.IconCorn1)));
            else if (upgradeSecond[0] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_2, Settings.costMarketCorn2, true, res.getHudTexture(HUDTexture.IconCorn2)));
            if(upgradeFirst[1] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1, Settings.costMarketMeat1, true, res.getHudTexture(HUDTexture.IconMeat1)));
            else if (upgradeSecond[1] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 1, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2, Settings.costMarketMeat2, true, res.getHudTexture(HUDTexture.IconMeat2)));
            if(upgradeFirst[2] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_1, Settings.costMarketStone1, true, res.getHudTexture(HUDTexture.IconStone1)));
            else if (upgradeSecond[2] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 2, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_2, Settings.costMarketStone2, true, res.getHudTexture(HUDTexture.IconStone2)));
            if(upgradeFirst[3] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1, Settings.costMarketWood1, true, res.getHudTexture(HUDTexture.IconWood1)));
            else if (upgradeSecond[3] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 3, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2, Settings.costMarketWood2, true, res.getHudTexture(HUDTexture.IconWood2)));
            if(upgradeFirst[4] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 4, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_1, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_1, Settings.costMarketOre1, true, res.getHudTexture(HUDTexture.IconOre1)));
            else if (upgradeSecond[4] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 4, this, Strings.PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_2, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_2, Settings.costMarketOre2, true, res.getHudTexture(HUDTexture.IconOre2)));
        }

        public override SourceAll getUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber)
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
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MarketBuilding, Strings.PROMPT_TITLE_WANT_TO_BUILD_MARKET, Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_MARKET, Settings.costMarket, true, GameResources.Inst().getHudTexture(HUDTexture.IconMarket));
        }
    }
}
