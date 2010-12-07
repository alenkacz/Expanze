using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay
{

    class FortModel : SpecialBuilding
    {
        int townID; // where is this building
        int hexaID;

        public FortModel(int townID, int hexaID)
        {
            this.townID = townID;
            this.hexaID = hexaID;
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
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.showPrompt(mod, Strings.PROMPT_TITLE_WANT_TO_BUILD_FORT, true);
            win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_CAPTURE, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_CAPTURE, Settings.costFortCapture, res.getHudTexture(HUDTexture.IconFortCapture)));
            win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, this, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_DESTROY_HEXA, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_DESTROY_HEXA, Settings.costFortDestroyHexa, res.getHudTexture(HUDTexture.IconFortHexa)));
            win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, this, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_SOURCES, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES, Settings.costFortSources, res.getHudTexture(HUDTexture.IconFortSources)));
            win.addPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, this, Strings.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_PARADE, Strings.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_PARADE, Settings.costFortParade, res.getHudTexture(HUDTexture.IconFortParade)));
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {
            upgradeCount--;
            switch (upgradeKind)
            {
                case UpgradeKind.FirstUpgrade:
                    switch (upgradeNumber)
                    {
                        case 0 :
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
            return new BuildingPromptItem(townID, hexaID, BuildingKind.FortBuilding, Strings.PROMPT_TITLE_WANT_TO_BUILD_FORT, Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_FORT, Settings.costFort, GameResources.Inst().getHudTexture(HUDTexture.IconFort));
        }
    }
}
