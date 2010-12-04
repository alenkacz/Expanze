using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class MarketSliderComponent : GameComponent
    {
        Texture2D sliderTexture;

        int sliderW = 24;
        int siderH = 36;

        Vector2 sliderPosition;
        Rectangle range;

       protected SpriteBatch spriteBatch;
        public Texture2D myButton;
        protected Vector2 spritePosition;
        protected Game myGame;
        protected SpriteFont gameFont;

        protected int width;
        protected int height;
        protected String texture;
        protected bool picked;

        protected Boolean pick;

        public MarketSliderComponent() { }

        public MarketSliderComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture)
        {
            pick = false;

            myGame = game;
            spritePosition.X = x;
            spritePosition.Y = y;
            gameFont = font;
            this.height = height;
            this.width = width;
            this.texture = texture;

            sliderPosition = new Vector2(x + 50, y);
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(myGame.GraphicsDevice);
            if (texture != null)
            {
                myButton = myGame.Content.Load<Texture2D>(texture);
            }

            sliderTexture = myGame.Content.Load<Texture2D>("slider");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, Boolean pick)
        {
            this.pick = pick;
            Draw(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            Color c;
            if (pick)
                c = Color.Black;
            else
                c = Color.White;
            if( myButton != null )
                spriteBatch.Draw(myButton,spritePosition, c);

            spriteBatch.Draw(sliderTexture, sliderPosition, c);

            spriteBatch.End();
        }

    }
}
