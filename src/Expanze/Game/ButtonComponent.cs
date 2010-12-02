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

        List<String> switchTexts = null;
        int activeText = 0;
        Texture2D pickedTexture;

        protected int mousex;
        protected int mousey;

        //type of the hexa - for materials buttons only
        HexaKind type;

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
            : base(game, x, y, font, width, height, texture)
        {
            this.type = HexaKind.Null;
            this.init(clickablePosition, x, y, width, height);
        }

        public ButtonComponent(Game game, int x, int y, Rectangle clickablePosition, SpriteFont font, int width, int height, String texture, HexaKind type)
            : base(game, x, y, font, width, height, texture)
        {
            this.type = type;
            this.init(clickablePosition, x, y, width, height);
        }

        public ButtonComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture, List<String> texts)
            : base(game, x, y, font, width, height, texture)
        {
            switchTexts = texts;
            this.init(new Rectangle(), x, y, width, height);
            spriteBatch = new SpriteBatch(myGame.GraphicsDevice);
        }

        private void init(Rectangle clickablePosition, int x, int y, int width, int height)
        {
            //clickablePos = new Rectangle(Settings.scaleW(clickablePosition.Left), Settings.scaleH(clickablePosition.Top), Settings.scaleW(clickablePosition.Right - clickablePosition.Left), Settings.scaleH(clickablePosition.Bottom - clickablePosition.Top));
            if (clickablePos.Top == clickablePos.Bottom && clickablePos.Bottom == 0)
            {
                //rectangle not specifies, whole place is clickable
                clickablePos = new Rectangle(Settings.scaleW(x), Settings.scaleH(y), width, height);
                pickedTexture = myGame.Content.Load<Texture2D>("HUD/ic_active");
            }
            else
            {
                clickablePos = clickablePosition;
            }
        }

        /// <summary>
        /// Returns type of the hexa which is button representing
        /// </summary>
        /// <returns>HexaKind</returns>
        public HexaKind getType()
        {
            return this.type;
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
                    if (Actions != null)
                        Actions(this, new PlayerIndexEventArgs(new PlayerIndex()));
                    
                    if( switchTexts != null )
                        nextText();
                    
                    pressed = true;
                }
            }

            if (pressed && ButtonState.Pressed != mouseState.LeftButton)
            {
                pressed = false;
            }
        }

        /// <summary>
        /// Switches between all texts
        /// </summary>
        public void nextText()
        {
            if (++activeText >= switchTexts.Count)
            {
                activeText = 0;
            }
        }

        /// <summary>
        /// Sets index of active text
        /// </summary>
        /// <param name="index">index of the text</param>
        public void setActiveTextIndex(int index)
        {
            if (index < switchTexts.Count)
            {
                activeText = index;
            }
        }

        /// <summary>
        /// Returns actually selected text
        /// </summary>
        public String getSelectedState()
        {
            return switchTexts.ElementAt(activeText);
        }

        public void setPosition(Vector2 v) 
        {
            spritePosition = v;
        }

        public void incrementPosition()
        {
            spritePosition.X += 30;
            clickablePos = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, width, height);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);

            Color c;
            c = Color.White;

            if (myButton != null)
            {
                spriteBatch.Draw(myButton, spritePosition, c);

                if (picked)
                {
                    spriteBatch.Draw(pickedTexture, spritePosition, c);
                }
            }

            if (switchTexts != null)
            {
                spriteBatch.DrawString(gameFont, switchTexts.ElementAt(activeText), spritePosition, Color.White);
            }

            spriteBatch.End();
        }
    }
}