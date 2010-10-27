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
        MouseState mouseState;

        int mousex;
        int mousey;

        //button still pressed
        bool pressed = false;

        public ButtonComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture) 
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
            base.Draw(gameTime);
            spriteBatch.Begin();

            if (ButtonState.Pressed == mouseState.LeftButton && !pressed)
            {

                if ((mousex > spritePosition.X && mousex < spritePosition.X + width) && (mousey < spritePosition.Y + height && mousey > spritePosition.Y))//identify mouse over x y posotions for the button
                {
                    GameMaster.getInstance().nextTurn();
                    pressed = true;
                }
            }

            if (pressed && ButtonState.Pressed != mouseState.LeftButton)
            {
                pressed = false;
            }

            spriteBatch.Draw(myButton, spritePosition, Color.White);
            spriteBatch.End();
        }
    }
}
