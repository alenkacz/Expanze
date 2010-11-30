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

        public override void setPromptWindow()
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.showPrompt("Tržiště", true);
            if(upgradeFirst[0] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, "Výměna obilí 1", "Popis", Settings.costMarketCorn1, res.getHudTexture(HUDTexture.IconCorn1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, "Výměna obilí 2", "Popis", Settings.costMarketCorn2, res.getHudTexture(HUDTexture.IconCorn2)));
            if(upgradeFirst[1] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, "Výměna masa 1", "Popis", Settings.costMarketMeat1, res.getHudTexture(HUDTexture.IconMeat1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 1, "Výměna masa 2", "Popis", Settings.costMarketMeat2, res.getHudTexture(HUDTexture.IconMeat2)));
            if(upgradeFirst[2] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, "Výměna kamenní 1", "Popis", Settings.costMarketStone1, res.getHudTexture(HUDTexture.IconStone1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 2, "Výměna kamenní 2", "Popis", Settings.costMarketStone2, res.getHudTexture(HUDTexture.IconStone2)));
            if(upgradeFirst[3] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, "Výměna dřeva 1", "Popis", Settings.costMarketWood1, res.getHudTexture(HUDTexture.IconWood1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 3, "Výměna dřeva 2", "Popis", Settings.costMarketWood2, res.getHudTexture(HUDTexture.IconWood2)));
            if(upgradeFirst[4] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 4, "Výměna rudy 1", "Popis", Settings.costMarketOre1, res.getHudTexture(HUDTexture.IconOre1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 4, "Výměna rudy 2", "Popis", Settings.costMarketOre2, res.getHudTexture(HUDTexture.IconOre2)));
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
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MarketBuilding, "Tržiště", "Trhy", Settings.costMarket, GameResources.Inst().getHudTexture(HUDTexture.IconMarket));
        }
    }
}
