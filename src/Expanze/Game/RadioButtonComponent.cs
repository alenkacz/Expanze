using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class RadioButtonComponent : GuiComponent
    {

        Color playerColor;

        MouseState mouseState;
        int mousex;
        int mousey;
        protected Rectangle clickablePos;
        bool pressed = false;

        private bool selected = false;
        Texture2D activeTexture;

        Rectangle range;

        public RadioButtonComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture)
            : base(game, x, y, font, width, height, texture) 
        {
            spriteBatch = new SpriteBatch(myGame.GraphicsDevice);
            this.clickablePos = new Rectangle(x, y, width, height);
            activeTexture = game.Content.Load<Texture2D>("radiobutton_active");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            mouseState = Mouse.GetState();

            mousex = mouseState.X;
            mousey = mouseState.Y;

            if (ButtonState.Pressed == mouseState.LeftButton && !pressed)
            {

                if ((mousex > clickablePos.Left && mousex < (clickablePos.Right)) && (mousey < (clickablePos.Bottom) && mousey > clickablePos.Top))//identify mouse over x y posotions for the button
                {
                    pressed = true;
                }
            }

            if (pressed && ButtonState.Pressed != mouseState.LeftButton)
            {
                pressed = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Color c;
            if (pick)
                c = Color.Black;
            else
                c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);

            spriteBatch.Draw(activeTexture, spritePosition, playerColor);

            if (this.selected)
            {
                spriteBatch.Draw(activeTexture, spritePosition, playerColor);
            }

            spriteBatch.End();
        }
    }
}
