using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze.Gameplay
{
    class MonasteryModel
    {
        public static BuildingPromptItem getPromptItemBuildMonastery(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MonasteryBuilding, "Klášter", "Klášter", Settings.costMonastery, GameResources.Inst().getHudTexture(HUDTexture.IconMonastery));
        }
    }
}
