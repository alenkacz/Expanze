using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay
{
    class PlayerPromptItem : PromptItem
    {
        Player player;

        public PlayerPromptItem(Player player, String title, String description, ISourceAll source, bool isSourceCost, Texture2D icon)
            : base(title, description, source, isSourceCost, icon)
        {
            this.player = player;

        }

        public override void DrawIcon(Microsoft.Xna.Framework.Vector2 iconPosition)
        {
            base.DrawIcon(iconPosition);
            Texture2D playerIcon = GameResources.Inst().getHudTexture(HUDTexture.PlayerColor);
            GameState.spriteBatch.Draw(playerIcon, new Vector2(iconPosition.X + getIcon().Width - playerIcon.Width - 5, iconPosition.Y + getIcon().Height - playerIcon.Height - 5), player.getColor());
        }

        public override void Execute()
        {
            base.Execute();
        }

        public override string TryExecute()
        {
            return base.TryExecute();
        }
    }

    class FortModel : SpecialBuilding
    {
        int townID; // where is this building
        int hexaID;
        bool playerPrompt;

        public FortModel(int townID, int hexaID)
        {
            this.townID = townID;
            this.hexaID = hexaID;
            playerPrompt = false;
        }

        public override Texture2D GetIconActive()
        {
            return GameResources.Inst().getHudTexture(HUDTexture.IconFortActive);
        }

        public override Texture2D GetIconPassive()
        {
            return GameResources.Inst().getHudTexture(HUDTexture.IconFort);
        }

        public override void setPromptWindow(PromptWindow.Mod mod)
        {
            if (!playerPrompt)
            {
                PromptWindow win = PromptWindow.Inst();
                GameResources res = GameResources.Inst();
                win.showPrompt(mod, Strings.PROMPT_TITLE_WANT_TO_BUILD_FORT, true);
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_CAPTURE, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_CAPTURE, Settings.costFortCapture, true, res.getHudTexture(HUDTexture.IconFortCapture)));
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, this, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_DESTROY_HEXA, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_DESTROY_HEXA, Settings.costFortDestroyHexa, true, res.getHudTexture(HUDTexture.IconFortHexa)));
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, this, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_SOURCES, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES, Settings.costFortSources, true, res.getHudTexture(HUDTexture.IconFortSources)));
                win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, this, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_PARADE, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_PARADE, Settings.costFortParade, true, res.getHudTexture(HUDTexture.IconFortParade)));
            }
            playerPrompt = false;
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            GameMaster gm = GameMaster.getInstance();

            upgradeCount--;
            switch (upgradeKind)
            {
                case UpgradeKind.FirstUpgrade:
                    switch (upgradeNumber)
                    {
                        case 0 :
                            break;

                        case 2 :
                            win.showPrompt(PromptWindow.Mod.Buyer, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_SOURCES, true);
                            for (int loop1 = 0; loop1 < gm.getPlayerCount(); loop1++)
                            {
                                if (gm.getPlayer(loop1) != gm.getActivePlayer())
                                {
                                    win.addPromptItem(new PlayerPromptItem(gm.getPlayer(loop1), gm.getPlayer(loop1).getName(), Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES_CHOISING_PLAYER, gm.getPlayer(loop1).GetSource(), false, res.getHudTexture(HUDTexture.IconTown)));
                                }
                            }
                            playerPrompt = true;
                            break;

                        case 3 :
                            GameMaster.getInstance().getActivePlayer().addPoints(Settings.pointsFortParade);
                            break;
                    }
                    upgradeFirst[upgradeNumber] = false;
                    break;
                case UpgradeKind.SecondUpgrade:
                    upgradeSecond[upgradeNumber] = false;
                    break;
            }
        }

        public override SourceAll getUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber)
        {
            if (upgradeKind == UpgradeKind.FirstUpgrade)
            {
                switch (upgradeNumber)
                {
                    case 0: return Settings.costFortCapture;
                    case 1: return Settings.costFortDestroyHexa;
                    case 2: return Settings.costFortSources;
                    case 3: return Settings.costFortParade;
                }
            }
            return new SourceAll(0);
        }

        public static BuildingPromptItem getPromptItemBuildFort(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.FortBuilding, Strings.PROMPT_TITLE_WANT_TO_BUILD_FORT, Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_FORT, Settings.costFort, true, GameResources.Inst().getHudTexture(HUDTexture.IconFort));
        }
    }
}
