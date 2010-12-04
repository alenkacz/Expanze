using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Expanze.Gameplay;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay
{
    class SourceBuildingModel : SpecialBuilding
    {
        int townID; // where is this building
        int hexaID;

        String titleBuilding;
        String upgrade1Title;
        String upgrade1Description;
        String upgrade2Title;
        String upgrade2Description;
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
            int buildingPos = town.findBuildingByHexaID(hexaID);
            HexaModel hexa = town.getHexa(buildingPos);


            upgrade1cost = new SourceAll(50, 0, 0, 40, 20);
            upgrade2cost = new SourceAll(0, 50, 50, 0, 40);
            switch (hexa.getType())
            {
                case HexaKind.Mountains:
                    buildingKind = SourceBuildingKind.Mine;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_MINE;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_MINE;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_MINE;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MINE;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MINE;
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconMine);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconMine);
                    break;
                case HexaKind.Forest:
                    buildingKind = SourceBuildingKind.Saw;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_SAW;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_SAW;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_SAW;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_SAW;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_SAW;
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconSaw);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconSaw);
                    break;
                case HexaKind.Cornfield:
                    buildingKind = SourceBuildingKind.Mill;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_MILL;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_MILL;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_MILL;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MILL;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MILL;
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconMill);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconMill);
                    break;
                case HexaKind.Pasture:
                    buildingKind = SourceBuildingKind.Stepherd;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_STEPHERD;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_STEPHERD;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_STEPHERD;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_STEPHERD;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_STEPHERD;
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconStepherd);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconStepherd);
                    break;
                case HexaKind.Stone:
                    buildingKind = SourceBuildingKind.Quarry;
                    titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_QUARRY;
                    upgrade1Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_1_QUARRY;
                    upgrade2Title = Strings.PROMPT_TITLE_WANT_TO_UPGRADE_2_QUARRY;
                    upgrade1Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_QUARRY;
                    upgrade2Description = Strings.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_QUARRY;
                    upgrade1icon = GameResources.Inst().getHudTexture(HUDTexture.IconQuarry);
                    upgrade2icon = GameResources.Inst().getHudTexture(HUDTexture.IconQuarry);
                    break;
            }
        }

        public override void setPromptWindow()
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.showPrompt(titleBuilding, true);
            if (upgradeFirst[0] == false)
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, upgrade1Title, upgrade1Description, upgrade1cost, upgrade1icon));
            else
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, upgrade2Title, upgrade2Description, upgrade2cost, upgrade2icon));
        }

        public override BuyingUpgradeError CanActivePlayerBuyUpgrade(UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.getInstance();
            Player activePlayer = gm.getActivePlayer();

            if (activePlayer.GetSourceBuildingUpgrade(buildingKind) == UpgradeKind.NoUpgrade ||
                (activePlayer.GetSourceBuildingUpgrade(buildingKind) == UpgradeKind.FirstUpgrade &&
                 upgradeKind == UpgradeKind.SecondUpgrade))
            {
                return BuyingUpgradeError.NoUpgrade;
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
