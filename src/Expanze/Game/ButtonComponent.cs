using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class ButtonComponent : GuiComponent
    {
        SpriteBatch spriteBatch;
        Texture2D myButton;
        Vector2 spritePosition;
        MouseState mouseState;
        Game myGame;
        SpriteFont gameFont;

        int mousex;
        int mousey;

        const int width = 150;
        const int height = 40;

        public ButtonComponent(Game game, int x, int y, SpriteFont font)
        {
            myGame = game;
            spritePosition.X = x;
            spritePosition.Y = y;
            gameFont = font;
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(myGame.GraphicsDevice);
            myButton = myGame.Content.Load<Texture2D>("button");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            mouseState = Mouse.GetState();

            mousex = mouseState.X;
            mousey = mouseState.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();

            if (ButtonState.Pressed == mouseState.LeftButton)
            {

                if ((mousex > spritePosition.X && mousex < spritePosition.X + width) && (mousey < spritePosition.Y + height && mousey > spritePosition.Y))//identify mouse over x y posotions for the button
                {
                    spriteBatch.DrawString(gameFont, "You click OK", new Vector2(100.0f, 50.0f), Color.YellowGreen);
                }
            }

            spriteBatch.Draw(myButton, spritePosition, Color.White);
            spriteBatch.End();
        }
    }
}
