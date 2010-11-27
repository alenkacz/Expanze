using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.Gameplay
{

    class FortModel
    {
        public static BuildingPromptItem getPromptItem(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, "Pevnost", "Pevnost", Settings.costFort, GameResources.Inst().getHudTexture(GameResources.HUD_ICON_FORT));
        }
    }
}
