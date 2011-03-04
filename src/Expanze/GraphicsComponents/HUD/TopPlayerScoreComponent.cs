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
    class TopPlayerScoreComponent : GuiComponent
    {
        Texture2D textureLeft;
        Texture2D textureRight;
        Texture2D textureMiddle;
        Texture2D textureColor;
        Texture2D textureMedaile;

        SpriteFont font = Settings.Game.Content.Load<SpriteFont>("playername");
        //SpriteBatch spriteBatch;

        Vector2 positionLeft;
        Vector2 positionRight;
        Vector2 positionMiddle;
        Vector2 positionName;
        Vector2 positionColor;
        Vector2 positionTotalPoints;

        int widthMiddle;
        //int widthScore = 100;
        const int medalWidth = 51;
        const int space = 20;
        const int rightSize = 79;
        const int leftSize = 11;
        const int middleSize = 6;

        //protected Boolean pick;

        public TopPlayerScoreComponent() 
        {
            widthMiddle = roundMiddleWidth(getPlayerNameWidth() + 2*space);

            positionLeft = new Vector2((int)Settings.maximumResolution.X - (rightSize + leftSize + widthMiddle), 0);
            positionRight = new Vector2((int)Settings.maximumResolution.X - rightSize, 0);
            positionMiddle = new Vector2((int)Settings.maximumResolution.X - (rightSize + widthMiddle), 0);

            positionName = new Vector2((int)Settings.maximumResolution.X - (rightSize - space + widthMiddle), 0);
            positionTotalPoints = new Vector2((int)Settings.maximumResolution.X - (rightSize + 2*space), 50);
            positionColor = new Vector2((int)Settings.maximumResolution.X - (rightSize - space), 6);
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Settings.Game.GraphicsDevice);

            textureLeft = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-left");
            textureRight = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-right");
            textureMiddle = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-middle");
            textureMedaile = Settings.Game.Content.Load<Texture2D>("HUD/score_medaile");
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

            foreach (Player p in GameMaster.Inst().GetPlayers())
            {
                if (font.MeasureString(p.GetName()).X > size)
                {
                    size = (int)font.MeasureString(p.GetName()).X;
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
            GameMaster gMaster = GameMaster.Inst();

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

            Player player = gMaster.GetTargetPlayer();

            Vector2 positionScore = new Vector2(positionTotalPoints.X - font.MeasureString(player.GetPoints().ToString()).X, positionTotalPoints.Y);

            // draw texts
            spriteBatch.DrawString(font, player.GetName(), positionName, Color.White);
            spriteBatch.DrawString(font, player.GetPoints().ToString(), positionScore, Color.White);
            spriteBatch.DrawString(font, " / " + GameMaster.Inst().GetGameSettings().getPoints(), positionTotalPoints, Color.White);

            // draw player color
            spriteBatch.Draw(textureColor, positionColor, player.GetColor());

            //draw medal
            spriteBatch.Draw(textureMedaile, new Vector2(positionScore.X - medalWidth - space,positionScore.Y), Color.White);

            spriteBatch.End();
        }

    }
}
