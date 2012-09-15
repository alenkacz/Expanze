using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Expanze.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Utils.Music;

namespace Expanze.Gameplay
{
    class SourceBuildingModel : SpecialBuilding
    {
        int townID; // where is this building
        int hexaID;
        UpgradeKind upgrade;

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
        SoundEnum sound;
        SourceBuildingKind buildingKind;


        public SourceBuildingModel(Player playerOwner, int townID, int hexaID) : base(playerOwner)
        {
            this.townID = townID;
            this.hexaID = hexaID;
            upgrade = UpgradeKind.NoUpgrade;
            
            TownModel town = GameState.map.GetTownByID(townID);
            int buildingPos = town.FindBuildingByHexaID(hexaID);
            HexaModel hexa = town.GetHexa(buildingPos);

            SourceAll sourceNormal = new SourceAll(0);
            int amountNormal = hexa.GetStartSource();
            switch (hexa.GetKind())
            {
                case HexaKind.Forest:
                    sourceNormal = new SourceAll(0, 0, 0, amountNormal, 0);
                    break;

                case HexaKind.Stone:
                    sourceNormal = new SourceAll(0, 0, amountNormal, 0, 0);
                    break;

                case HexaKind.Cornfield:
                    sourceNormal = new SourceAll(amountNormal, 0, 0, 0, 0);
                    break;

                case HexaKind.Pasture:
                    sourceNormal = new SourceAll(0, amountNormal, 0, 0, 0);
                    break;

                case HexaKind.Mountains:
                    sourceNormal = new SourceAll(0, 0, 0, 0, amountNormal);
                    break;
            }
            playerOwner.AddCollectSources(sourceNormal, new SourceAll(0));

            upgrade1cost = new SourceAll(0);
            upgrade2cost = new SourceAll(0);
            switch (hexa.GetKind())
            {
                case HexaKind.Mountains:
                    buildingKind = SourceBuildingKind.Mine;
                    sound = SoundEnum.mine;
                    titleBuilding = Strings.Inst().GetString(TextEnum.PROMT_TITLE_WANT_TO_BUILD_MINE);
                    upgrade1Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_MINE);
                    upgrade2Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_MINE);
                    upgrade1Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MINE);
                    upgrade2Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MINE);
                    upgrade0iconActive = GameResources.Inst().GetHudTexture(HUDTexture.IconMineActive);
                    upgrade0icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMine);
                    upgrade1icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMine1);
                    upgrade2icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMine2);
                    break;
                case HexaKind.Forest:
                    sound = SoundEnum.sawmill;
                    buildingKind = SourceBuildingKind.Saw;
                    titleBuilding = Strings.Inst().GetString(TextEnum.PROMT_TITLE_WANT_TO_BUILD_SAW);
                    upgrade1Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_SAW);
                    upgrade2Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_SAW);
                    upgrade1Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_SAW);
                    upgrade2Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_SAW);
                    upgrade0iconActive = GameResources.Inst().GetHudTexture(HUDTexture.IconSawActive);
                    upgrade0icon = GameResources.Inst().GetHudTexture(HUDTexture.IconSaw);
                    upgrade1icon = GameResources.Inst().GetHudTexture(HUDTexture.IconSaw1);
                    upgrade2icon = GameResources.Inst().GetHudTexture(HUDTexture.IconSaw2);
                    break;
                case HexaKind.Cornfield:
                    sound = SoundEnum.windmill;
                    buildingKind = SourceBuildingKind.Mill;
                    titleBuilding = Strings.Inst().GetString(TextEnum.PROMT_TITLE_WANT_TO_BUILD_MILL);
                    upgrade1Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_MILL);
                    upgrade2Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_MILL);
                    upgrade1Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MILL);
                    upgrade2Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MILL);
                    upgrade0iconActive = GameResources.Inst().GetHudTexture(HUDTexture.IconMillActive);
                    upgrade0icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMill);
                    upgrade1icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMill1);
                    upgrade2icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMill2);
                    break;
                case HexaKind.Pasture:
                    sound = SoundEnum.stepherd;
                    buildingKind = SourceBuildingKind.Stepherd;
                    titleBuilding = Strings.Inst().GetString(TextEnum.PROMT_TITLE_WANT_TO_BUILD_STEPHERD);
                    upgrade1Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_STEPHERD);
                    upgrade2Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_STEPHERD);
                    upgrade1Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_STEPHERD);
                    upgrade2Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_STEPHERD);
                    upgrade0iconActive = GameResources.Inst().GetHudTexture(HUDTexture.IconStepherdActive);
                    upgrade0icon = GameResources.Inst().GetHudTexture(HUDTexture.IconStepherd);
                    upgrade1icon = GameResources.Inst().GetHudTexture(HUDTexture.IconStepherd1);
                    upgrade2icon = GameResources.Inst().GetHudTexture(HUDTexture.IconStepherd2);
                    break;
                case HexaKind.Stone:
                    sound = SoundEnum.quarry;
                    buildingKind = SourceBuildingKind.Quarry;
                    titleBuilding = Strings.Inst().GetString(TextEnum.PROMT_TITLE_WANT_TO_BUILD_QUARRY);
                    upgrade1Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_QUARRY);
                    upgrade2Title = Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_QUARRY);
                    upgrade1Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_QUARRY);
                    upgrade2Description = Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_QUARRY);
                    upgrade0iconActive = GameResources.Inst().GetHudTexture(HUDTexture.IconQuarryActive);
                    upgrade0icon = GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry);
                    upgrade1icon = GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry1);
                    upgrade2icon = GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry2);
                    break;
            }
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {
            upgrade = upgradeKind;
            SetPromptWindow(PromptWindow.Mod.Buyer, true);
        }

        public UpgradeKind GetUpgrade() { return owner.GetMonasteryUpgrade(buildingKind);}

        public override Texture2D GetIconActive()
        {
            return upgrade0iconActive;
        }

        public override Texture2D GetIconPassive()
        {
            switch (GetUpgrade())
            {
                case UpgradeKind.NoUpgrade: return upgrade0icon;
                case UpgradeKind.FirstUpgrade: return upgrade1icon;
                case UpgradeKind.SecondUpgrade: return upgrade2icon;
                default: return upgrade0icon;
            }
        }

        public override void SetPromptWindow(PromptWindow.Mod mod, bool silent)
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.Show(mod, titleBuilding, true);
            upgrade = GetUpgrade();
            if (upgrade == UpgradeKind.NoUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, upgrade1Title, upgrade1Description, upgrade1cost, true, upgrade1icon));
            else if (upgrade == UpgradeKind.FirstUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, this, upgrade2Title, upgrade2Description, upgrade2cost, true, upgrade2icon));
            else
            {
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, this, titleBuilding, Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_ALL_UPGRADES_USED), new SourceAll(0), true, upgrade2icon));
           
                //win.AddPromptItem(new PromptItem(titleBuilding, Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_ALL_UPGRADES_USED), new SourceAll(0), false, false, upgrade2icon));
            }
            MusicManager.Inst().PlaySound(sound);
        }

        public override BuyingUpgradeError CanActivePlayerBuyUpgrade(UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.Inst();
            Player activePlayer = gm.GetActivePlayer();

            if (activePlayer.GetMonasteryUpgrade(buildingKind) == UpgradeKind.NoUpgrade ||
                (activePlayer.GetMonasteryUpgrade(buildingKind) == UpgradeKind.FirstUpgrade &&
                 upgradeKind == UpgradeKind.SecondUpgrade))
            {
                return BuyingUpgradeError.NoUpgrade;
            }

            if (upgradeKind == UpgradeKind.SecondUpgrade)
            {
                if (upgrade == UpgradeKind.NoUpgrade)
                {
                    GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.YOU_DONT_HAVE_FIRST_UPGRADE));
                    return BuyingUpgradeError.YouDontHaveFirstUpgrade;
                }
                if (upgrade == UpgradeKind.SecondUpgrade)
                {
                    GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.ERROR_HAVE_SECOND_UPGRADE));
                    return BuyingUpgradeError.YouAlreadyHaveSecondUpgrade;
                }
            }

            if (!GetUpgradeCost(upgradeKind, upgradeNumber).HasPlayerSources(activePlayer))
            {
                GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.ERROR_NO_SOURCES));
                return BuyingUpgradeError.NoSources;
            }

            return BuyingUpgradeError.OK;
        }

        public override SourceAll GetUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber)
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
