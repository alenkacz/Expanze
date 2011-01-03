﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay
{
    class MonasteryModel : SpecialBuilding, IMonastery
    {
        int townID; // where is this building
        int hexaID;

        public MonasteryModel(Player playerOwner, int townID, int hexaID) : base(playerOwner)
        {
            this.townID = townID;
            this.hexaID = hexaID;
        }

        public override Texture2D GetIconActive()
        {
            return GameResources.Inst().GetHudTexture(HUDTexture.IconMonasteryActive);
        }

        public override Texture2D GetIconPassive()
        {
            return GameResources.Inst().GetHudTexture(HUDTexture.IconMonastery);
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster.Inst().GetActivePlayer().SetSourceBuildingUpdate(upgradeKind, upgradeNumber);
        }

        public override void SetPromptWindow(PromptWindow.Mod mod)
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.Show(mod, Strings.PROMPT_TITLE_WANT_TO_BUILD_MONASTERY, true);
            
            if(upgradeFirst[0] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_MILL, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MILL, Settings.costMonasteryCorn1, true, res.GetHudTexture(HUDTexture.IconMill1)));
            else if (upgradeSecond[0] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_MILL, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MILL, Settings.costMonasteryCorn2, true, res.GetHudTexture(HUDTexture.IconMill2)));
            if(upgradeFirst[1] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_STEPHERD, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_STEPHERD, Settings.costMonasteryMeat1, true, res.GetHudTexture(HUDTexture.IconStepherd1)));
            else if (upgradeSecond[1] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 1, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_STEPHERD, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_STEPHERD, Settings.costMonasteryMeat2, true, res.GetHudTexture(HUDTexture.IconStepherd2)));
            if(upgradeFirst[2] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_QUARRY, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_QUARRY, Settings.costMonasteryStone1, true, res.GetHudTexture(HUDTexture.IconQuarry1)));
            else if (upgradeSecond[2] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 2, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_QUARRY, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_QUARRY, Settings.costMonasteryStone2, true, res.GetHudTexture(HUDTexture.IconQuarry2)));
            if(upgradeFirst[3] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_SAW, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_SAW, Settings.costMonasteryWood1, true, res.GetHudTexture(HUDTexture.IconSaw1)));
            else if (upgradeSecond[3] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 3, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_SAW, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_SAW, Settings.costMonasteryWood2, true, res.GetHudTexture(HUDTexture.IconSaw2)));
            if(upgradeFirst[4] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 4, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_MINE, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MINE, Settings.costMonasteryOre1, true, res.GetHudTexture(HUDTexture.IconMine1)));
            else if (upgradeSecond[4] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 4, this, Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_MINE, Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MINE, Settings.costMonasteryOre2, true, res.GetHudTexture(HUDTexture.IconMine2)));
        }

        public override SourceAll GetUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber)
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

        public static BuildingPromptItem GetPromptItemBuildMonastery(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MonasteryBuilding, Strings.PROMPT_TITLE_WANT_TO_BUILD_MONASTERY, Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_MONASTERY, Settings.costMonastery, true, GameResources.Inst().GetHudTexture(HUDTexture.IconMonastery));
        }

        public bool InventUpgrade(SourceKind source)
        {
            UpgradeKind kind;
            if (!upgradeFirst[(int)source])
                kind = UpgradeKind.FirstUpgrade;
            else
                kind = UpgradeKind.SecondUpgrade;

            return GameState.map.GetMapController().BuyUpgradeInSpecialBuilding(townID, hexaID, kind, (int) source) == BuyingUpgradeError.OK;
        }
    }
}
