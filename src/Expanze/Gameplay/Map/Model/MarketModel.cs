using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze.Gameplay
{
    class MarketModel
    {
        public static BuildingPromptItem getPromptItemBuildMarket(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.MarketBuilding, "Tržiště", "Trhy", Settings.costMarket, GameResources.Inst().getHudTexture(HUDTexture.IconMarket));
        }
    }
}
