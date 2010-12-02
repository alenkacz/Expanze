using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class MarketSliderComponent : GuiComponent
    {

        //space between texts in HUD of materials
        const int space = 150;
        int start = 60;

        List<ButtonComponent> playerButtons = new List<ButtonComponent>();
        Texture2D playerColorTexture;
        Color playerColor;
        ButtonComponent slider;

        Rectangle range;

        public MarketSliderComponent(Game game, int x, int y, SpriteFont font)
            : base(game, x, y, font, 400, 50, "slider_market") 
        {
            this.range = new Rectangle(x, y, 400, 50);
            slider = new ButtonComponent(game,x + 50,y,new Rectangle(),font,24,36,"slider");
            slider.Actions += SliderMove;
        }

        public override void Initialize()
        {
            slider.Initialize();
            slider.LoadContent();
        }

        public override void UnloadContent()
        {
            slider.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            slider.Update(gameTime);
        }

        /// <summary>
        /// Event handler for change button
        /// </summary>
        void SliderMove(object sender, PlayerIndexEventArgs e)
        {
            slider.incrementPosition();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Color c;
            if (pick)
                c = Color.Black;
            else
                c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);

            if (pick)
            {
                spriteBatch.End();
                return;
            }

            spriteBatch.Draw(myButton, spritePosition, playerColor);
            slider.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
