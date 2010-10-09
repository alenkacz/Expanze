using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace tutorial
{
    class CustomCursor : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D myCursor;
        MouseState currMouseState;
        Vector2 spritePosition;

        public CustomCursor(Game game):base(game)
        {
   
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            myCursor = Game.Content.Load<Texture2D>("cursor1");
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
