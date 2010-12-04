﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class RadioButtonComponent : GameComponent
    {

        Color playerColor;
        protected SpriteBatch spriteBatch;
        protected Vector2 spritePosition;
        protected Game myGame;

        protected Rectangle clickablePos;
        bool pressed = false;

        private bool selected = false;
        Texture2D activeTexture;
        Texture2D bgTexture;

        Rectangle range;

        public RadioButtonComponent(Game game, int x, int y, SpriteFont font, int width, int height)
        {
            myGame = game;
            this.clickablePos = new Rectangle(Settings.scaleW(x), Settings.scaleH(y), width, height);
            spritePosition = new Vector2(x, y);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(myGame.GraphicsDevice);
            activeTexture = myGame.Content.Load<Texture2D>("radiobutton_active");
            bgTexture = myGame.Content.Load<Texture2D>("radiobutton_bg");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool isInRange(int mousex, int mousey)
        {
            if ((mousex > clickablePos.Left && mousex < (clickablePos.Right)) && (mousey < (clickablePos.Bottom) && mousey > clickablePos.Top))//identify mouse over x y posotions for the button
            {
                return true;
            }

            return false;
        }

        public void setSelected(bool b) 
        {
            this.selected = b;
        }

        public void clicked()
        {
            this.selected = !this.selected;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Color c; 
            c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);

            spriteBatch.Draw(bgTexture, spritePosition, Color.White);

            if (this.selected)
            {
                spriteBatch.Draw(activeTexture, new Vector2((int)spritePosition.X + 3, (int)spritePosition.Y + 6), Color.White);
            }

            spriteBatch.End();
        }
    }
}
