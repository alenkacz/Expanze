using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay
{
    class MonasteryModel : SpecialBuilding
    {
        int townID; // where is this building
        int hexaID;

        public MonasteryModel(int townID, int hexaID)
        {
            this.townID = townID;
            this.hexaID = hexaID;
        }

        public override Texture2D GetIconActive()
        {
            return GameResources.Inst().getHudTexture(HUDTexture.IconMonasteryActive);
        }

        public override Texture2D GetIconPassive()
        {
            return GameResources.Inst().getHudTexture(HUDTexture.IconMonastery);
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster.getInstance().getActivePlayer().SetSourceBuildingUpdate(upgradeKind, upgradeNumber);
        }

        public override void setPromptWindow()
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.showPrompt(Strings.PROMPT_TITLE_WANT_TO_BUILD_MONASTERY, true);
            if(upgradeFirst[0] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, "Vylepšení mlýna 1", "Popis", Settings.costMonasteryCorn1, res.getHudTexture(HUDTexture.IconMill1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, "Zdokonalení mlýna", "Popis", Settings.costMonasteryCorn2, res.getHudTexture(HUDTexture.IconMill2)));
            if(upgradeFirst[1] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, "Vylepšení pastevce 1", "Popis", Settings.costMonasteryMeat1, res.getHudTexture(HUDTexture.IconStepherd1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 1, "Zdokonalení pastevce 2", "Popis", Settings.costMonasteryMeat2, res.getHudTexture(HUDTexture.IconStepherd2)));
            if(upgradeFirst[2] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, "Vylepšení lomu 1", "Popis", Settings.costMonasteryStone1, res.getHudTexture(HUDTexture.IconQuarry1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 2, "Zdokonalení lomu 2", "Popis", Settings.costMonasteryStone2, res.getHudTexture(HUDTexture.IconQuarry2)));
            if(upgradeFirst[3] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, "Vylepšení pily 1", "Popis", Settings.costMonasteryWood1, res.getHudTexture(HUDTexture.IconSaw1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 3, "Zdokonalení pily 2", "Popis", Settings.costMonasteryWood2, res.getHudTexture(HUDTexture.IconSaw2)));
            if(upgradeFirst[4] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 4, "Vylepšení dolu 1", "Popis", Settings.costMonasteryOre1, res.getHudTexture(HUDTexture.IconMine1)));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 4, "Zdokonalení dolu 2", "Popis", Settings.costMonasteryOre2, res.getHudTexture(HUDTexture.IconMine2)));
        }

        public override SourceAll getUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber)
        {
            switch (upgradeKind)
            {
                case UpgradeKind.FirstUpgrade :
                    switch (upgradeNumber)
                    {
                        case 0: return Settings.costMonasteryCorn1;
                        case 1: return Settings.costMonasteryMeat1;
                        case 2: return Settings.costMonasteryStone1;
                        case 3: return Settings.costMonasteryWood1;
                        case 4: return Settings.costMonasteryOre1;
                    }
                    break;
                case UpgradeKind.SecondUpgrade :
                    switch (upgradeNumber)
                    {
                        case 0: return Settings.costMonasteryCorn2;
                        case 1: return Settings.costMonasteryMeat2;
                        case 2: return Settings.costMonasteryStone2;
                        case 3: return Settings.costMonasteryWood2;
                        case 4: return Settings.costMonasteryOre2;
                    }
                    break;
            }

            return new SourceAll(0);
        }

        public static BuildingPromptItem getPromptItemBuildMonastery(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MonasteryBuilding, Strings.PROMPT_TITLE_WANT_TO_BUILD_MONASTERY, Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_MONASTERY, Settings.costMonastery, GameResources.Inst().getHudTexture(HUDTexture.IconMonastery));
        }
    }
}
