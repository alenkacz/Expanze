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
        //protected bool picked;

        SourceKind fromKind = SourceKind.Null;
        SourceKind toKind = SourceKind.Null;

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
                if (fromKind != SourceKind.Null && toKind != SourceKind.Null)
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
            clickablePos = new Rectangle(Settings.scaleW(sliderPosition.X), Settings.scaleH(sliderPosition.Y), sliderW, sliderH);
            this.fromKind = SourceKind.Null;
            this.toKind = SourceKind.Null;
            this.fromTypeCount = 0;
            this.fromConvertedCount = 0;
            this.toTypeCount = 0;
            this.toConvertedCount = 0;
        }

        public void moveSliderToStart()
        {
            sliderPosition.X = spritePosition.X;
            clickablePos = new Rectangle(Settings.scaleW(sliderPosition.X), Settings.scaleH(sliderPosition.Y), sliderW, sliderH);
            this.fromTypeCount = GameMaster.Inst().GetActivePlayer().GetMaterialNumber(fromKind);
            this.fromConvertedCount = fromTypeCount;
            this.toTypeCount = GameMaster.Inst().GetActivePlayer().GetMaterialNumber(toKind);
            this.toConvertedCount = toTypeCount;
        }

        private void moveSlider(int pos)
        {
            //if (Settings.scaleW(pos) < (range.Right) && Settings.scaleW(pos) > range.Left)

            if(Settings.scaleW(pos) > range.Right)
                pos = Settings.UnScaleW(range.Right);
            if (Settings.scaleW(pos) < range.Left)
                pos = Settings.UnScaleW(range.Left) + 1;
            {
                int unit = getSliderUnit();
                /// NEED TO BE FIXED //
                /// unit should be 1/2 for example //
                if (unit == 0)
                    unit = 1;
                
                //int converted = (int)(spritePosition.X - pos)/unit;

                int converted = (int) Math.Round(((pos - spritePosition.X) / (float)width) * getMaxToKindSourcesToConvert());

                if (converted < 0) converted = -converted;

                if ((this.fromTypeCount - converted * GameMaster.Inst().GetActivePlayer().GetConversionRate(fromKind)) >= 0)
                {
                    this.fromConvertedCount = this.fromTypeCount - converted * GameMaster.Inst().GetActivePlayer().GetConversionRate(fromKind);
                    this.toConvertedCount = this.toTypeCount + converted;

                    sliderPosition.X = pos;
                }
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

        private int getMaxToKindSourcesToConvert()
        {
            return GameMaster.Inst().GetActivePlayer().GetMaterialNumber(fromKind) / GameMaster.Inst().GetActivePlayer().GetConversionRate(fromKind);
        }

        private int getSliderUnit()
        {
            return width / getMaxToKindSourcesToConvert();
        }

        public void setFromType(SourceKind k)
        {
            this.fromKind = k;
            this.fromTypeCount = GameMaster.Inst().GetActivePlayer().GetMaterialNumber(fromKind);
            this.fromConvertedCount = fromTypeCount;
            moveSliderToStart();
        }

        public void setToType(SourceKind k)
        {
            toKind = k;
            this.toTypeCount = GameMaster.Inst().GetActivePlayer().GetMaterialNumber(toKind);
            this.toConvertedCount = toTypeCount;
            moveSliderToStart();
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

            if (fromKind != SourceKind.Null)
            {
                spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalMedium), fromConvertedCount.ToString(), new Vector2(spritePosition.X - 100, spritePosition.Y + Settings.scaleH(50)), Color.White);
            }

            if (toKind != SourceKind.Null)
            {
                spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalMedium), toConvertedCount.ToString(), new Vector2(spritePosition.X + width + 50, spritePosition.Y + Settings.scaleH(50)), Color.White);
            }

            spriteBatch.End();
        }

    }
}
