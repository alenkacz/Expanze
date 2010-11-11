using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class MenuButtonComponent : GuiComponent
    {

        protected MouseState mouseState;

        protected int mousex;
        protected int mousey;

        //button still pressed
        protected bool pressed = false;

        public MenuButtonComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture) 
            : base(game,x,y,font,width,height,texture) {}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            mouseState = Mouse.GetState();

            mousex = mouseState.X;
            mousey = mouseState.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);

            if (ButtonState.Pressed == mouseState.LeftButton && !pressed)
            {

                if ((mousex > spritePosition.X && mousex < spritePosition.X + width) && (mousey < spritePosition.Y + height && mousey > spritePosition.Y))//identify mouse over x y posotions for the button
                {
                    GameMaster.getInstance().setPausedNew(true);
                    pressed = true;
                }
            }

            if (pressed && ButtonState.Pressed != mouseState.LeftButton)
            {
                pressed = false;
            }

            Color c;
            if (pick)
                c = Color.Black;
            else
                c = Color.White;
            spriteBatch.Draw(myButton, spritePosition, c);
            spriteBatch.End();
        }
    }
}
