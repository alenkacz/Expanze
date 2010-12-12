﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze
{
    class TopPlayerScoreComponent : GuiComponent
    {
        Texture2D textureLeft;
        Texture2D textureRight;
        Texture2D textureMiddle;
        Texture2D textureColor;

        SpriteFont font = Settings.Game.Content.Load<SpriteFont>("playername");
        SpriteBatch spriteBatch;

        Vector2 positionLeft;
        Vector2 positionRight;
        Vector2 positionMiddle;
        Vector2 positionName;
        Vector2 positionColor;
        Vector2 positionScore;

        int widthMiddle;
        int widthScore = 100;
        const int space = 20;
        const int rightSize = 79;
        const int leftSize = 11;
        const int middleSize = 6;

        protected Boolean pick;

        public TopPlayerScoreComponent() 
        {
            widthMiddle = roundMiddleWidth(getPlayerNameWidth() + 2*space + widthScore);

            positionLeft = new Vector2((int)Settings.maximumResolution.X - (rightSize + leftSize + widthMiddle), 0);
            positionRight = new Vector2((int)Settings.maximumResolution.X - rightSize, 0);
            positionMiddle = new Vector2((int)Settings.maximumResolution.X - (rightSize + widthMiddle), 0);

            positionName = new Vector2((int)Settings.maximumResolution.X - (rightSize - space - widthScore + widthMiddle), 0);
            positionScore = new Vector2((int)Settings.maximumResolution.X - (rightSize - space/2 + widthMiddle), 0);
            positionColor = new Vector2((int)Settings.maximumResolution.X - (rightSize - space), 6);
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Settings.Game.GraphicsDevice);

            textureLeft = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-left");
            textureRight = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-right");
            textureMiddle = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-middle");
            textureColor = Settings.Game.Content.Load<Texture2D>("pcolor");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Measures names of all players and returns the biggest one
        /// </summary>
        /// <returns></returns>
        public int getPlayerNameWidth()
        {
            int size = 0;

            foreach (Player p in GameMaster.getInstance().getPlayers())
            {
                if (font.MeasureString(p.getName()).X > size)
                {
                    size = (int)font.MeasureString(p.getName()).X;
                }
            }

            return size;
        }

        /// <summary>
        /// Width must be multiplication of middle texture width
        /// </summary>
        /// <param name="w">counted width</param>
        /// <returns>right width</returns>
        private int roundMiddleWidth(int w)
        {
            if ((w % middleSize) != 0)
            {
                int res = w / middleSize;
                return (res + 1) * middleSize;
            }

            return w;
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);

            Color c = Color.White;
            GameMaster gMaster = GameMaster.getInstance();

            // draw textures
            spriteBatch.Draw(textureLeft, positionLeft, c);
            spriteBatch.Draw(textureRight, positionRight, c);

            // drawing the middle part - simulation of CSS repeat-x
            Vector2 posMiddleDraw = positionMiddle;

            for (int i = 0; i < (widthMiddle / middleSize); i++)
            {
                spriteBatch.Draw(textureMiddle, posMiddleDraw, c);
                posMiddleDraw.X += middleSize;
            }

            // draw texts
            spriteBatch.DrawString(font, gMaster.getActivePlayer().getName(), positionName, Color.White);
            spriteBatch.DrawString(font, gMaster.getActivePlayer().getPoints().ToString(), positionScore, Color.White);

            // draw player color
            spriteBatch.Draw(textureColor, positionColor, gMaster.getActivePlayer().getColor());

            spriteBatch.End();
        }

    }
}