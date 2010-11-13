using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze
{
    class MarketComponent : GuiComponent
    {
        public static bool isActive = false;
        private static MarketComponent instance = null;

        private MarketComponent() 
        :base(Settings.Game,100,10,GameState.gameFont,300,300,"market_bg"){ }

        public static MarketComponent getInstance() {
            if (instance == null)
            {
                instance = new MarketComponent();
            }

            return instance;
        }
    }
}
