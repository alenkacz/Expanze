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
        Texture2D textureMedal;

        SpriteFont font = Settings.Game.Content.Load<SpriteFont>("playername");
        //SpriteBatch spriteBatch;

        Vector2 positionLeft;
        Vector2 positionRight;
        Vector2 positionMiddle;
        Vector2 positionName;
        Vector2 positionColor;
        Vector2 positionTotalPoints;
        Vector2 positionScore;

        int widthMiddle;
        //int widthScore = 100;
        const int medalWidth = 51;
        const int space = 30;
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
            
            positionColor = new Vector2((int)Settings.maximumResolution.X - (rightSize - space), 6);
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Settings.Game.GraphicsDevice);

            textureLeft = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-left");
            textureRight = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-right");
            textureMiddle = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-middle");
            textureMedal = Settings.Game.Content.Load<Texture2D>("HUD/score_medaile");
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

            Color c = (pick) ? Color.Black : Color.White;
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
            DrawAllPoints();

            // draw texts
            spriteBatch.DrawString(font, player.GetName(), positionName, c);
            // draw player color
            spriteBatch.Draw(textureColor, positionColor, (pick) ? c : player.GetColor());

            spriteBatch.End();
        }

        private void DrawPoints(Texture2D medal, int myPoints, int goalPoints, int pointKind)
        {
            positionTotalPoints = new Vector2((int)Settings.maximumResolution.X - (rightSize + 2 * space), 65 + pointKind * 65);
            positionScore = new Vector2(positionTotalPoints.X - 20, positionTotalPoints.Y);
            //draw medal
            spriteBatch.Draw(medal, new Vector2(positionScore.X - textureMedal.Width - space, positionScore.Y + 5 - textureMedal.Height / 2), Color.White);
            spriteBatch.DrawString(font, myPoints.ToString(), positionScore, Color.White);
            spriteBatch.DrawString(font, " / " + goalPoints, positionTotalPoints, Color.White);
        }

        private Texture2D DrawAllPoints()
        {
            int pointKind = 0;
            Player player = GameMaster.Inst().GetTargetPlayer();
            if (Settings.pointsMedal > 0)
                DrawPoints(textureMedal, player.GetPoints(PlayerPoints.Medal), Settings.pointsMedal, pointKind++);

            if (Settings.pointsTown > 0)
                DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Town), player.GetPoints(PlayerPoints.Town), Settings.pointsTown, pointKind++);

            if (Settings.pointsRoad > 0)
                DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Road), player.GetPoints(PlayerPoints.Road), Settings.pointsRoad, pointKind++);

            if (Settings.pointsMill > 0) DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Mill), player.GetPoints(PlayerPoints.Mill), Settings.pointsMill, pointKind++);
            if (Settings.pointsMine > 0) DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Mine), player.GetPoints(PlayerPoints.Mine), Settings.pointsMine, pointKind++);
            if (Settings.pointsQuarry > 0) DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Quarry), player.GetPoints(PlayerPoints.Quarry), Settings.pointsQuarry, pointKind++);
            if (Settings.pointsSaw > 0) DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Saw), player.GetPoints(PlayerPoints.Saw), Settings.pointsSaw, pointKind++);
            if (Settings.pointsStepherd > 0) DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Stepherd), player.GetPoints(PlayerPoints.Stepherd), Settings.pointsStepherd, pointKind++);
            if (Settings.pointsFort > 0) DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Fort), player.GetPoints(PlayerPoints.Fort), Settings.pointsFort, pointKind++);
            if (Settings.pointsMarket > 0) DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Market), player.GetPoints(PlayerPoints.Market), Settings.pointsMarket, pointKind++);
            if (Settings.pointsMonastery > 0) DrawPoints(GameMaster.Inst().GetMedaileIcon(Building.Monastery), player.GetPoints(PlayerPoints.Monastery), Settings.pointsMonastery, pointKind++);
            if (Settings.pointsFortParade > 0) DrawPoints(GameResources.Inst().GetHudTexture(HUDTexture.IconFortParade), player.GetPoints(PlayerPoints.FortParade), Settings.pointsFortParade, pointKind++);
            if (Settings.pointsFortCapture > 0) DrawPoints(GameResources.Inst().GetHudTexture(HUDTexture.IconFortCapture), player.GetPoints(PlayerPoints.FortCaptureHexa), Settings.pointsFortCapture, pointKind++);
            if (Settings.pointsFortSteal > 0) DrawPoints(GameResources.Inst().GetHudTexture(HUDTexture.IconFortSources), player.GetPoints(PlayerPoints.FortStealSources), Settings.pointsFortSteal, pointKind++);
            if (Settings.pointsMarketLvl1 > 0) DrawPoints(GameResources.Inst().GetHudTexture(HUDTexture.IconOre1), player.GetPoints(PlayerPoints.LicenceLvl1), Settings.pointsMarketLvl1, pointKind++);
            if (Settings.pointsUpgradeLvl1 > 0) DrawPoints(GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry1), player.GetPoints(PlayerPoints.UpgradeLvl1), Settings.pointsUpgradeLvl1, pointKind++);
            if (Settings.pointsMarketLvl2 > 0) DrawPoints(GameResources.Inst().GetHudTexture(HUDTexture.IconOre2), player.GetPoints(PlayerPoints.LicenceLvl2), Settings.pointsMarketLvl2, pointKind++);
            if (Settings.pointsUpgradeLvl2 > 0) DrawPoints(GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry2), player.GetPoints(PlayerPoints.UpgradeLvl2), Settings.pointsUpgradeLvl2, pointKind++);

            return textureMedal;
        }

    }
}
