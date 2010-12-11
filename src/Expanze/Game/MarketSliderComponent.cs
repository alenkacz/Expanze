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
    class MarketSliderComponent : GameComponent
    {
        Texture2D sliderTexture;

        int sliderW = 24;
        int sliderH = 36;

        Vector2 sliderPosition;
        Rectangle range;

        protected int mousex;
        protected int mousey;
        protected MouseState mouseState;
        bool pressed = false;
        Rectangle clickablePos;

       protected SpriteBatch spriteBatch;
        public Texture2D myButton;
        protected Vector2 spritePosition;
        protected Game myGame;
        protected SpriteFont gameFont;

        protected int width;
        protected int height;
        protected String texture;
        protected bool picked;

        HexaKind fromType = HexaKind.Null;
        HexaKind toType = HexaKind.Null;

        int fromTypeCount = 0;
        int toTypeCount = 0;

        int fromConvertedCount = 0;
        int toConvertedCount = 0;

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

            sliderPosition = new Vector2(x, y - 10);
            clickablePos = new Rectangle(Settings.scaleW(x), Settings.scaleH(y - 10), sliderW, sliderH);
            range = new Rectangle(Settings.scaleW(x), Settings.scaleH(y), Settings.scaleW(width), Settings.scaleH(height));
        }

        public override void LoadContent()
        {
            base.LoadContent();

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

            if (pressed && ButtonState.Pressed == mouseState.LeftButton)
            {
                if (fromType != HexaKind.Null && toType != HexaKind.Null)
                {
                    // disable moving when both materials are not selected
                    moveSlider(((int)(mousex / Settings.spriteScale.M11)));
                }
            }

            if (pressed && ButtonState.Pressed != mouseState.LeftButton)
            {
                pressed = false;
                clickablePos = new Rectangle(Settings.scaleW(sliderPosition.X), Settings.scaleH(sliderPosition.Y), sliderW, sliderH);
            }
        }

        public void resetSlider() 
        {
            sliderPosition.X = spritePosition.X;
            this.fromType = HexaKind.Null;
            this.toType = HexaKind.Null;
            this.fromTypeCount = 0;
            this.fromConvertedCount = 0;
            this.toTypeCount = 0;
            this.toConvertedCount = 0;
        }

        private void moveSlider(int pos)
        {
            if (Settings.scaleW(pos) < (range.Right - Settings.scaleW(24)) && Settings.scaleW(pos) > range.Left)
            {
                int unit = getSliderUnit();
                int converted = (int)(spritePosition.X - pos)/unit;

                if (converted < 0) converted = -converted;

                this.fromConvertedCount = this.fromTypeCount - converted * GameMaster.getInstance().getActivePlayer().getConversionRate(fromType);
                this.toConvertedCount = this.toTypeCount + converted;

                sliderPosition.X = pos;
            }
        }

        public int getConvertedFrom()
        {
            return this.fromConvertedCount;
        }

        public int getConvertedTo() 
        {
            return toConvertedCount;
        }

        private int getSliderUnit()
        {
            int count = GameMaster.getInstance().getActivePlayer().getMaterialNumber(fromType)/GameMaster.getInstance().getActivePlayer().getConversionRate(fromType);
            return width / count;
        }

        public void setFromType(HexaKind k)
        {
            this.fromType = k;
            this.fromTypeCount = GameMaster.getInstance().getActivePlayer().getMaterialNumber(fromType);
            this.fromConvertedCount = fromTypeCount;
        }

        public void setToType(HexaKind k)
        {
            toType = k;
            this.toTypeCount = GameMaster.getInstance().getActivePlayer().getMaterialNumber(toType);
            this.toConvertedCount = toTypeCount;
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

            if (fromType != HexaKind.Null)
            {
                spriteBatch.DrawString(GameState.gameFont, fromConvertedCount.ToString(), new Vector2(spritePosition.X - 100, spritePosition.Y + Settings.scaleH(50)), Color.White);
            }

            if (toType != HexaKind.Null)
            {
                spriteBatch.DrawString(GameState.gameFont, toConvertedCount.ToString(), new Vector2(spritePosition.X + width + 50, spritePosition.Y + Settings.scaleH(50)), Color.White);
            }

            spriteBatch.End();
        }

    }
}
