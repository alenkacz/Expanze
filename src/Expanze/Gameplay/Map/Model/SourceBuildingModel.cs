using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Expanze.Gameplay;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay
{
    class SourceBuildingModel : SpecialBuilding, ISourceBuilding
    {
        int townID; // where is this building
        int hexaID;

        String titleBuilding;
        String upgrade1Title;
        String upgrade1Description;
        String upgrade2Title;
        String upgrade2Description;
        Texture2D upgrade0iconActive;
        Texture2D upgrade0icon;
        Texture2D upgrade1icon;
        Texture2D upgrade2icon;
        SourceAll upgrade1cost;
        SourceAll upgrade2cost;
        SourceBuildingKind buildingKind;


        public SourceBuildingModel(int townID, int hexaID)
        {
            this.townID = townID;
            this.hexaID = hexaID;

            Town town = GameState.map.GetTownByID(townID);
            int buildingPos = town.FindBuildingByHexaID(hexaID);
            HexaModel hexa = town.GetHexa(buildingPos);


            upgrade1cost = new SourceAll(50, 0, 0, 40, 20);
            upgrade2cost = new SourceAll(0, 50, 50, 0, 40);
            switch (hexa.getKind())
            {
                case HexaKind.Mountains:
                    buildingKind = SourceBuildingKind.Mine;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_MINE;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_MINE;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_MINE;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MINE;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MINE;
                    upgrade0iconActive = GameResources.Inst().getHudTexture(HUDTexture.IconMineActive);
                    upgrade0icon = GameResources.Inst().getHudTexture(HUDTexture.IconMine);
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconMine1);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconMine2);
                    break;
                case HexaKind.Forest:
                    buildingKind = SourceBuildingKind.Saw;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_SAW;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_SAW;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_SAW;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_SAW;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_SAW;
                    upgrade0iconActive = GameResources.Inst().getHudTexture(HUDTexture.IconSawActive);
                    upgrade0icon = GameResources.Inst().getHudTexture(HUDTexture.IconSaw);
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconSaw1);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconSaw2);
                    break;
                case HexaKind.Cornfield:
                    buildingKind = SourceBuildingKind.Mill;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_MILL;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_MILL;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_MILL;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MILL;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MILL;
                    upgrade0iconActive = GameResources.Inst().getHudTexture(HUDTexture.IconMillActive);
                    upgrade0icon = GameResources.Inst().getHudTexture(HUDTexture.IconMill);
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconMill1);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconMill2);
                    break;
                case HexaKind.Pasture:
                    buildingKind = SourceBuildingKind.Stepherd;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_STEPHERD;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_STEPHERD;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_STEPHERD;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_STEPHERD;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_STEPHERD;
                    upgrade0iconActive = GameResources.Inst().getHudTexture(HUDTexture.IconStepherdActive);
                    upgrade0icon = GameResources.Inst().getHudTexture(HUDTexture.IconStepherd);
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconStepherd1);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconStepherd2);
                    break;
                case HexaKind.Stone:
                    buildingKind = SourceBuildingKind.Quarry;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_QUARRY;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_QUARRY;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_QUARRY;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_QUARRY;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_QUARRY;
                    upgrade0iconActive = GameResources.Inst().getHudTexture(HUDTexture.IconQuarryActive);
                    upgrade0icon = GameResources.Inst().getHudTexture(HUDTexture.IconQuarry);
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconQuarry1);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconQuarry2);
                    break;
            }
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {

        }

        public override Texture2D GetIconActive()
        {
            return upgrade0iconActive;
        }

        public override Texture2D GetIconPassive()
        {
            if (!upgradeFirst[0])
                return upgrade0icon;
            else if (upgradeFirst[0] && !upgradeSecond[0])
                return upgrade1icon;
            else
                return upgrade2icon;
        }

        public override void setPromptWindow(PromptWindow.Mod mod)
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.Show(mod, titleBuilding, true);
            if (upgradeFirst[0] == false)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, upgrade1Title, upgrade1Description, upgrade1cost, true, upgrade1icon));
            else
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, this, upgrade2Title, upgrade2Description, upgrade2cost, true, upgrade2icon));
        }

        public override BuyingUpgradeError CanActivePlayerBuyUpgrade(UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.Inst();
            Player activePlayer = gm.GetActivePlayer();

            if (activePlayer.GetSourceBuildingUpgrade(buildingKind) == UpgradeKind.NoUpgrade ||
                (activePlayer.GetSourceBuildingUpgrade(buildingKind) == UpgradeKind.FirstUpgrade &&
                 upgradeKind == UpgradeKind.SecondUpgrade))
            {
                return BuyingUpgradeError.NoUpgrade;
            }

            if (upgradeKind == UpgradeKind.SecondUpgrade)
            {
                if (upgradeFirst[upgradeNumber] == false)
                    return BuyingUpgradeError.YouDontHaveFirstUpgrade;
                if (upgradeSecond[upgradeNumber] == true)
                    return BuyingUpgradeError.YouAlreadyHaveSecondUpgrade;
            }     

            if (!getUpgradeCost(upgradeKind, upgradeNumber).HasPlayerSources(activePlayer))
                return BuyingUpgradeError.NoSources;

            return BuyingUpgradeError.OK;
        }

        public override SourceAll getUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber)
        {
            switch (upgradeKind)
            {
                case UpgradeKind.FirstUpgrade:
                    switch (upgradeNumber)
                    {
                        case 0: return upgrade1cost;
                    }
                    break;
                case UpgradeKind.SecondUpgrade:
                    switch (upgradeNumber)
                    {
                        case 0: return upgrade2cost;
                    }
                    break;
            }

            return new SourceAll(0);
        }
    }
}
