using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class MarketComponent : GuiComponent
    {
        public static bool isActive = false;
        private static MarketComponent instance = null;
        List<GuiComponent> content = new List<GuiComponent>();
        Rectangle range;

        private MarketComponent()
        :base(Settings.Game,100,10,GameState.gameFont,300,300,"market_bg")
        {
            this.range = new Rectangle(100,10,300,300);
        }

        public static MarketComponent getInstance() {
            if (instance == null)
            {
                instance = new MarketComponent();
            }

            return instance;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            ButtonComponent obili_button = new ButtonComponent(Settings.Game, Settings.scaleW(range.Left + 40), (int)(range.Top + 40), new Rectangle(), GameState.gameFont, Settings.scaleW(120), Settings.scaleH(120), "obili-button");
            this.content.Add(obili_button);

            foreach (GuiComponent g in content)
            {
                g.Initialize();
                g.LoadContent();
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            foreach (GuiComponent g in content)
            {
                g.UnloadContent();
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            foreach( GuiComponent g in content ) {
                g.Update(gameTime);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (GuiComponent g in content)
            {
                g.Update(gameTime);
            }
        }
    }
}
