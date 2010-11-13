using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze
{
    class ButtonComponent : GuiComponent
    {
        protected MouseState mouseState;

        protected Rectangle clickablePos;

        protected int mousex;
        protected int mousey;

        //button still pressed
        protected bool pressed = false;

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Actions;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Actions != null)
                Actions(this, new PlayerIndexEventArgs(playerIndex));
        }


        public ButtonComponent(Game game, int x, int y, Rectangle clickablePosition, SpriteFont font, int width, int height, String texture) 
            : base(game,x,y,font,width,height,texture) {
                //clickablePos = new Rectangle(Settings.scaleW(clickablePosition.Left), Settings.scaleH(clickablePosition.Top), Settings.scaleW(clickablePosition.Right - clickablePosition.Left), Settings.scaleH(clickablePosition.Bottom - clickablePosition.Top));
                clickablePos = clickablePosition;
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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);

            if (ButtonState.Pressed == mouseState.LeftButton && !pressed)
            {

                if ((mousex > clickablePos.Left && mousex < (clickablePos.Right)) && (mousey < (clickablePos.Bottom) && mousey > clickablePos.Top))//identify mouse over x y posotions for the button
                {
                    if (Actions != null)
                        Actions(null, new PlayerIndexEventArgs(new PlayerIndex()));
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
