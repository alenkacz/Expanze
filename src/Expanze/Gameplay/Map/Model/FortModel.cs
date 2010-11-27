using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze.Gameplay
{

    class FortModel
    {
        public static BuildingPromptItem getPromptItem(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.FortBuilding, "Pevnost", "Pevnost", Settings.costFort, GameResources.Inst().getHudTexture(GameResources.HUD_ICON_FORT));
        }
    }
}
