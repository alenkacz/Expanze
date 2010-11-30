using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze.Gameplay
{

    class FortModel
    {
        public static void setPromptWindowToFort()
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            win.showPrompt("Pevnost", true);
            win.addPromptItem(new PromptItem("Obsaď pole", "Popis", Settings.costFortCapture, res.getHudTexture(HUDTexture.IconFortCapture)));
            win.addPromptItem(new PromptItem("Ponič pole", "Popis", Settings.costFortHexa, res.getHudTexture(HUDTexture.IconFortHexa)));
            win.addPromptItem(new PromptItem("Znič suroviny", "Popis", Settings.costFortSources, res.getHudTexture(HUDTexture.IconFortSources)));
            win.addPromptItem(new PromptItem("Armádní přehlídka", "Popis", Settings.costFortParade, res.getHudTexture(HUDTexture.IconFortParade)));
        }

        public static BuildingPromptItem getPromptItemBuildFort(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.FortBuilding, "Pevnost", "Pevnost", Settings.costFort, GameResources.Inst().getHudTexture(HUDTexture.IconFort));
        }
    }
}
