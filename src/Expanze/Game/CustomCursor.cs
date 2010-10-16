using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class CustomCursor : GameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D myCursor;
        MouseState currMouseState;
        Vector2 spritePosition;
        Game myGame;

        public CustomCursor(Game game)
        {
            myGame = game;
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(myGame.GraphicsDevice);
            myCursor = myGame.Content.Load<Texture2D>("cursor");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            currMouseState = Mouse.GetState();
            spritePosition.X = currMouseState.X;
            spritePosition.Y = currMouseState.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.Draw(myCursor, spritePosition, Color.White);
            spriteBatch.End();
        }
    }
}
