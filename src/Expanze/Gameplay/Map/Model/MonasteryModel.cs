using System;
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
            Player player = GameMaster.Inst().GetActivePlayer();
            player.SetSourceBuildingUpdate(upgradeKind, upgradeNumber);
            if (!player.GetIsAI())
            {
                int tempItem = PromptWindow.Inst().GetActiveItem();
                SetPromptWindow(PromptWindow.Mod.Buyer);
                PromptWindow.Inst().SetActiveItem(tempItem);
            }
            else
            {
                Message.Inst().Show(player.GetName() + " vynalezl pokrok", player.GetName() + " vynalezl " + GetUpgradeKindString(upgradeKind) + " pro větší zisky surovin z " + GetUpgradeBuildingName(upgradeNumber) + ".", GetUpgradeIcon(upgradeKind, upgradeNumber));
            }
        }

        private Texture2D GetUpgradeIcon(UpgradeKind kind, int upgradeNumber)
        {
            switch (kind)
            {
                case UpgradeKind.FirstUpgrade:
                    switch (upgradeNumber)
                    {
                        case 0: return GameResources.Inst().GetHudTexture(HUDTexture.IconMill1);
                        case 1: return GameResources.Inst().GetHudTexture(HUDTexture.IconStepherd1);
                        case 2: return GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry1);
                        case 3: return GameResources.Inst().GetHudTexture(HUDTexture.IconWood1);
                        case 4: return GameResources.Inst().GetHudTexture(HUDTexture.IconMine1);
                    }
                    break;
                case UpgradeKind.SecondUpgrade:
                    switch (upgradeNumber)
                    {
                        case 0: return GameResources.Inst().GetHudTexture(HUDTexture.IconMill2);
                        case 1: return GameResources.Inst().GetHudTexture(HUDTexture.IconStepherd2);
                        case 2: return GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry2);
                        case 3: return GameResources.Inst().GetHudTexture(HUDTexture.IconWood2);
                        case 4: return GameResources.Inst().GetHudTexture(HUDTexture.IconMine2);
                    }
                    break;
            }

            return null;
        }

        private String GetUpgradeBuildingName(int number)
        {
            switch (number)
            {
                case 0: return "větrného mlýna";
                case 1: return "chatrče pastevce";
                case 2: return "kamenného lomu";
                case 3: return "pily";
                case 4: return "dolu na rudu";
            }
            return "";
        }

        private String GetUpgradeKindString(UpgradeKind kind)
        {
            switch (kind)
            {
                case UpgradeKind.FirstUpgrade: return "první pokrok";
                case UpgradeKind.SecondUpgrade: return "druhý pokrok";
            }
            return "";
        }

        public override void SetPromptWindow(PromptWindow.Mod mod)
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.Show(mod, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUILD_MONASTERY), true);
            
            if(owner.GetMonasteryUpgrade(SourceBuildingKind.Mill) == UpgradeKind.NoUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_MILL), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MILL), Settings.costMonasteryCorn1, true, res.GetHudTexture(HUDTexture.IconMill1)));
            else if (owner.GetMonasteryUpgrade(SourceBuildingKind.Mill) == UpgradeKind.FirstUpgrade && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 0, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_MILL), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MILL), Settings.costMonasteryCorn2, true, res.GetHudTexture(HUDTexture.IconMill2)));

            if (owner.GetMonasteryUpgrade(SourceBuildingKind.Stepherd) == UpgradeKind.NoUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_STEPHERD), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_STEPHERD), Settings.costMonasteryMeat1, true, res.GetHudTexture(HUDTexture.IconStepherd1)));
            else if (owner.GetMonasteryUpgrade(SourceBuildingKind.Stepherd) == UpgradeKind.FirstUpgrade && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 1, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_STEPHERD), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_STEPHERD), Settings.costMonasteryMeat2, true, res.GetHudTexture(HUDTexture.IconStepherd2)));
            
            if (owner.GetMonasteryUpgrade(SourceBuildingKind.Quarry) == UpgradeKind.NoUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_QUARRY), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_QUARRY), Settings.costMonasteryStone1, true, res.GetHudTexture(HUDTexture.IconQuarry1)));
            else if (owner.GetMonasteryUpgrade(SourceBuildingKind.Quarry) == UpgradeKind.FirstUpgrade && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 2, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_QUARRY), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_QUARRY), Settings.costMonasteryStone2, true, res.GetHudTexture(HUDTexture.IconQuarry2)));
            
            if (owner.GetMonasteryUpgrade(SourceBuildingKind.Saw) == UpgradeKind.NoUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_SAW), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_SAW), Settings.costMonasteryWood1, true, res.GetHudTexture(HUDTexture.IconSaw1)));
            else if (owner.GetMonasteryUpgrade(SourceBuildingKind.Saw) == UpgradeKind.FirstUpgrade && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 3, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_SAW), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_SAW), Settings.costMonasteryWood2, true, res.GetHudTexture(HUDTexture.IconSaw2)));

            if (owner.GetMonasteryUpgrade(SourceBuildingKind.Mine) == UpgradeKind.NoUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 4, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_1_MINE), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MINE), Settings.costMonasteryOre1, true, res.GetHudTexture(HUDTexture.IconMine1)));
            else if (owner.GetMonasteryUpgrade(SourceBuildingKind.Mine) == UpgradeKind.FirstUpgrade && !Settings.banSecondUpgrade)
                win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.SecondUpgrade, 4, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_UPGRADE_2_MINE), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MINE), Settings.costMonasteryOre2, true, res.GetHudTexture(HUDTexture.IconMine2)));

            if (win.GetItemCount() == 0)
            {
                win.AddPromptItem(new PromptItem(Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUILD_MONASTERY), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_ALL_UPGRADES_INVENTED), new SourceAll(0), false, false, GameResources.Inst().GetHudTexture(HUDTexture.IconMonastery)));
            }
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
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MonasteryBuilding, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUILD_MONASTERY), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUILD_MONASTERY), Settings.costMonastery, true, GameResources.Inst().GetHudTexture(HUDTexture.IconMonastery));
        }

        public bool InventUpgrade(SourceBuildingKind source)
        {
            UpgradeKind kind;
            switch (owner.GetMonasteryUpgrade(source))
            {
                case UpgradeKind.NoUpgrade: kind = UpgradeKind.FirstUpgrade; break;
                case UpgradeKind.FirstUpgrade: kind = UpgradeKind.SecondUpgrade; break;
                default : return false;
            }

            return GameState.map.GetMapController().BuyUpgradeInSpecialBuilding(townID, hexaID, kind, (int) source);
        }

        public MonasteryError CanInventUpgrade(SourceBuildingKind source)
        {
            UpgradeKind kind;
            switch (owner.GetMonasteryUpgrade(source))
            {
                case UpgradeKind.NoUpgrade: kind = UpgradeKind.FirstUpgrade; break;
                case UpgradeKind.FirstUpgrade: kind = UpgradeKind.SecondUpgrade; break;
                default: kind = UpgradeKind.NoUpgrade; break;
            }

            if (kind == UpgradeKind.SecondUpgrade &&
               Settings.banSecondUpgrade)
                return MonasteryError.BanSecondUpgrade;

            BuyingUpgradeError error = GameState.map.GetMapController().CanBuyUpgradeInSpecialBuilding(townID, hexaID, kind, (int)source);

            switch (error)
            {
                case BuyingUpgradeError.YouAlreadyHaveSecondUpgrade:
                    GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.ERROR_HAVE_SECOND_UPGRADE));
                    return MonasteryError.HaveSecondUpgrade;
                case BuyingUpgradeError.NoSources:
                    GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.ERROR_NO_SOURCES));
                    return MonasteryError.NoSources;
                case BuyingUpgradeError.MaxUpgrades:
                    GameState.map.GetMapController().SetLastError(Strings.Inst().GetString(TextEnum.ERROR_MAX_UPGRADES));
                    return MonasteryError.MaxUpgrades;
                case BuyingUpgradeError.OK: return MonasteryError.OK;
            }

            return MonasteryError.OK;
        }

        public override bool GetFreePlaceForUpgrade()
        {
            return owner.UpgradeFreeSlot > 0;
        }

        #region IMonastery Members


        public int GetFreeSlot()
        {
            return upgradeMax - upgradeCount;
        }

        #endregion
    }
}
