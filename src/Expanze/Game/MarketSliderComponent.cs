using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

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
            clickablePos = new Rectangle(Settings.scaleW(x + 50), Settings.scaleH(y-10), sliderW, sliderH);
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
                moveSlider(((int)(mousex / Settings.spriteScale.M11)));
            }

            if (pressed && ButtonState.Pressed != mouseState.LeftButton)
            {
                pressed = false;
                clickablePos = new Rectangle(Settings.scaleW(sliderPosition.X), Settings.scaleH(sliderPosition.Y), sliderW, sliderH);
            }
        }

        private void moveSlider(int pos)
        {
            if (Settings.scaleW(pos) < range.Right && Settings.scaleW(pos) > range.Left)
            {
                sliderPosition.X = pos;
            }
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

            spriteBatch.End();
        }

    }
}
