using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class GuiComponent : GameComponent
    {
        protected SpriteBatch spriteBatch;
        protected Texture2D myButton;
        protected Vector2 spritePosition;
        protected Game myGame;
        protected SpriteFont gameFont;

        protected int width;
        protected int height;
        protected String texture;

        public GuiComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture)
        {
            myGame = game;
            spritePosition.X = x;
            spritePosition.Y = y;
            gameFont = font;
            this.height = height;
            this.width = width;
            this.texture = texture;
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(myGame.GraphicsDevice);
            myButton = myGame.Content.Load<Texture2D>(texture);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            spriteBatch.Draw(myButton,spritePosition, Color.White);
            spriteBatch.End();
        }


    }
}
