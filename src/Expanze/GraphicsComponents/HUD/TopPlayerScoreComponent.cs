using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using CorePlugin;
using Expanze.Utils;

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

        Vector2 positionTurn;
        Vector2 positionLeft;
        Vector2 positionRight;
        Vector2 positionMiddle;
        Vector2 positionName;
        Vector2 positionColor;
        Vector2 positionTotalPoints;
        Vector2 positionScore;

        int widthMiddle;
        //int widthScore = 100;
        const int turnWidth = 40;
        const int medalWidth = 51;
        const int space = 34;
        const int rightSize = 79;
        const int leftSize = 11;
        const int middleSize = 6;
        const int border = 8;
        //protected Boolean pick;

        public TopPlayerScoreComponent() 
        {
            widthMiddle = roundMiddleWidth(getPlayerNameWidth() + 2*space);

            positionTurn = new Vector2((int)Settings.maximumResolution.X / 2 - turnWidth / 2, border);
            positionLeft = new Vector2((int)Settings.maximumResolution.X - (rightSize + leftSize + widthMiddle) - border, border);
            positionRight = new Vector2((int)Settings.maximumResolution.X - rightSize - border, border);
            positionMiddle = new Vector2((int)Settings.maximumResolution.X - (rightSize + widthMiddle) - border, border);

            positionName = new Vector2((int)Settings.maximumResolution.X - (rightSize - space + widthMiddle) - border, border);
            
            positionColor = new Vector2((int)Settings.maximumResolution.X - (rightSize - space) - border, 6 + border);
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

            spriteBatch.Draw(textureLeft, new Vector2(positionTurn.X, positionTurn.Y), c);
            //positionTurn.X += textureLeft.Width +50;
            int number = 2;
            spriteBatch.Draw(textureRight, new Vector2(positionTurn.X + textureLeft.Width + (number - 2) * textureMiddle.Width, positionTurn.Y), c);

            for (int loop1 = 0; loop1 < number; loop1++)
            {
                spriteBatch.Draw(textureMiddle, new Vector2(positionTurn.X + textureLeft.Width + loop1 * textureMiddle.Width, positionTurn.Y), c);
            }

            
            Player player = gMaster.GetTargetPlayer();
            DrawAllPoints();

            // draw texts
            spriteBatch.DrawString(font, player.GetName(), positionName, c);
            // draw player color
            spriteBatch.Draw(textureColor, positionColor, (pick) ? c : player.GetColor());

            int turnsLeft = gMaster.GetTurnsLeft();
            if (!pick && turnsLeft <= 10)
            {
                if (turnsLeft <= 5)
                    c = Color.Red;
                else
                    c = Color.Yellow;
            }
            
            int tempWidth = (int)font.MeasureString(" " + turnsLeft).X;
            spriteBatch.DrawString(font, " " + turnsLeft, new Vector2(positionTurn.X + 38 - tempWidth / 2, border), c);


            spriteBatch.End();
        }

        MouseState mouse;

        int helpX, helpY;
        String helpText1, helpText2;
        
        private int DrawPoints(String text1, String text2, Texture2D medal, int myPoints, int goalPoints, int pointKind)
        {
            if (goalPoints - myPoints > 0)
            {
                float goalHeight = 65;
                positionTotalPoints = new Vector2((int)Settings.maximumResolution.X - (rightSize + 2 * space) + 60, 65 + border + pointKind * goalHeight);
                positionScore = new Vector2(positionTotalPoints.X - 20, positionTotalPoints.Y);
                //draw medal

                int x = (int) (positionScore.X - textureMedal.Width - space + 65);
                int y = (int) (positionScore.Y + 10 + border - textureMedal.Height / 2);
                spriteBatch.Draw(medal, new Vector2(x, y), (pick) ? Color.Black : Color.White);

                String pointString = (goalPoints - myPoints) + "";
                float pointWidth = font.MeasureString(pointString).X;
                positionScore = new Vector2(positionTotalPoints.X + 55 - pointWidth, positionTotalPoints.Y - 8);
                spriteBatch.DrawString(font, pointString, positionScore, Color.Black);
                positionScore = new Vector2(positionTotalPoints.X + 58 - pointWidth, positionTotalPoints.Y - 6);
                spriteBatch.DrawString(font, pointString, positionScore, (pick) ? Color.Black : Settings.colorMainText);

                Rectangle rect = new Rectangle(Settings.scaleW(x), Settings.scaleH(y), Settings.scaleW(medal.Width + 100), Settings.scaleH(goalHeight));
                if (rect.Contains(mouse.X, mouse.Y))
                {
                    helpX = x;
                    helpY = y + 17;
                    helpText1 = text1;
                    helpText2 = text2;
                }

                return pointKind + 1;
            } else
                return pointKind;
        }

        private Texture2D DrawAllPoints()
        {
            int pointKind = 0;
            Player player = GameMaster.Inst().GetTargetPlayer();
            mouse = Mouse.GetState();

            helpText1 = null;

            if (Settings.pointsTown > 0)
                pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_TOWN_PART1), Strings.Inst().GetString(TextEnum.GOAL_TOWN_PART2), GameMaster.Inst().GetMedaileIcon(Building.Town), player.GetPoints(PlayerPoints.Town), Settings.pointsTown, pointKind);

            if (Settings.pointsRoad > 0)
                pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_ROAD_PART1), Strings.Inst().GetString(TextEnum.GOAL_ROAD_PART2), GameMaster.Inst().GetMedaileIcon(Building.Road), player.GetPoints(PlayerPoints.Road), Settings.pointsRoad, pointKind);

            if (Settings.goalRoadID > 0)
                pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_ROADID_PART1), Strings.Inst().GetString(TextEnum.GOAL_ROADID_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconMedalRoadID), player.GetPoints(PlayerPoints.RoadID), Settings.goalRoadID, pointKind);
            if (Settings.goalTownID > 0)
                pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_TOWNID_PART1), Strings.Inst().GetString(TextEnum.GOAL_TOWNID_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconMedalTownID), player.GetPoints(PlayerPoints.TownID), Settings.goalTownID, pointKind);

            if (Settings.pointsMill > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_MILL_PART1), Strings.Inst().GetString(TextEnum.GOAL_MILL_PART2), GameMaster.Inst().GetMedaileIcon(Building.Mill), player.GetPoints(PlayerPoints.Mill), Settings.pointsMill, pointKind);
            if (Settings.pointsMine > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_MINE_PART1), Strings.Inst().GetString(TextEnum.GOAL_MINE_PART2), GameMaster.Inst().GetMedaileIcon(Building.Mine), player.GetPoints(PlayerPoints.Mine), Settings.pointsMine, pointKind);
            if (Settings.pointsQuarry > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_QUARRY_PART1), Strings.Inst().GetString(TextEnum.GOAL_QUARRY_PART2), GameMaster.Inst().GetMedaileIcon(Building.Quarry), player.GetPoints(PlayerPoints.Quarry), Settings.pointsQuarry, pointKind);
            if (Settings.pointsSaw > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_SAW_PART1), Strings.Inst().GetString(TextEnum.GOAL_SAW_PART2), GameMaster.Inst().GetMedaileIcon(Building.Saw), player.GetPoints(PlayerPoints.Saw), Settings.pointsSaw, pointKind);
            if (Settings.pointsStepherd > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_STEPHERD_PART1), Strings.Inst().GetString(TextEnum.GOAL_STEPHERD_PART2), GameMaster.Inst().GetMedaileIcon(Building.Stepherd), player.GetPoints(PlayerPoints.Stepherd), Settings.pointsStepherd, pointKind);
            if (Settings.pointsFort > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_FORT_PART1), Strings.Inst().GetString(TextEnum.GOAL_FORT_PART2), GameMaster.Inst().GetMedaileIcon(Building.Fort), player.GetPoints(PlayerPoints.Fort), Settings.pointsFort, pointKind);
            if (Settings.pointsMarket > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_MARKET_PART1), Strings.Inst().GetString(TextEnum.GOAL_MARKET_PART2), GameMaster.Inst().GetMedaileIcon(Building.Market), player.GetPoints(PlayerPoints.Market), Settings.pointsMarket, pointKind);
            if (Settings.pointsMonastery > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_MONASTERY_PART1), Strings.Inst().GetString(TextEnum.GOAL_MONASTERY_PART2), GameMaster.Inst().GetMedaileIcon(Building.Monastery), player.GetPoints(PlayerPoints.Monastery), Settings.pointsMonastery, pointKind);
            if (Settings.pointsFortParade > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_FORT_PARADE_PART1), Strings.Inst().GetString(TextEnum.GOAL_FORT_PARADE_PART2), textureMedal, player.GetPoints(PlayerPoints.FortParade), Settings.pointsFortParade, pointKind);
            if (Settings.pointsFortCapture > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_FORT_CAPTURE_PART1), Strings.Inst().GetString(TextEnum.GOAL_FORT_CAPTURE_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconFortCapture), player.GetPoints(PlayerPoints.FortCaptureHexa), Settings.pointsFortCapture, pointKind);
            if (Settings.pointsFortSteal > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_FORT_STEAL_PART1), Strings.Inst().GetString(TextEnum.GOAL_FORT_STEAL_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconFortSources), player.GetPoints(PlayerPoints.FortStealSources), Settings.pointsFortSteal, pointKind);
            if (Settings.pointsMarketLvl1 > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_LICENCE1_PART1), Strings.Inst().GetString(TextEnum.GOAL_LICENCE1_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconOre1), player.GetPoints(PlayerPoints.LicenceLvl1), Settings.pointsMarketLvl1, pointKind);
            if (Settings.pointsUpgradeLvl1 > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_UPGRADE1_PART1), Strings.Inst().GetString(TextEnum.GOAL_UPGRADE1_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry1), player.GetPoints(PlayerPoints.UpgradeLvl1), Settings.pointsUpgradeLvl1, pointKind);
            if (Settings.pointsMarketLvl2 > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_LICENCE2_PART1), Strings.Inst().GetString(TextEnum.GOAL_LICENCE2_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconOre2), player.GetPoints(PlayerPoints.LicenceLvl2), Settings.pointsMarketLvl2, pointKind);
            if (Settings.pointsUpgradeLvl2 > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_UPGRADE2_PART1), Strings.Inst().GetString(TextEnum.GOAL_UPGRADE2_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry2), player.GetPoints(PlayerPoints.UpgradeLvl2), Settings.pointsUpgradeLvl2, pointKind);
            if (Settings.pointsCorn > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_CORN_PART1), Strings.Inst().GetString(TextEnum.GOAL_CORN_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallCorn), player.GetCollectSourcesNormal().GetCorn(), Settings.pointsCorn, pointKind);
            if (Settings.pointsMeat > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_MEAT_PART1), Strings.Inst().GetString(TextEnum.GOAL_MEAT_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallMeat), player.GetCollectSourcesNormal().GetMeat(), Settings.pointsMeat, pointKind);
            if (Settings.pointsStone > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_STONE_PART1), Strings.Inst().GetString(TextEnum.GOAL_STONE_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallStone), player.GetCollectSourcesNormal().GetStone(), Settings.pointsStone, pointKind);
            if (Settings.pointsWood > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_WOOD_PART1), Strings.Inst().GetString(TextEnum.GOAL_WOOD_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallWood), player.GetCollectSourcesNormal().GetWood(), Settings.pointsWood, pointKind);
            if (Settings.pointsOre > 0) pointKind = DrawPoints(Strings.Inst().GetString(TextEnum.GOAL_ORE_PART1), Strings.Inst().GetString(TextEnum.GOAL_ORE_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallOre), player.GetCollectSourcesNormal().GetOre(), Settings.pointsOre, pointKind);

            if (helpText1 != null)
            {
                TextWrapping.DrawStringOnScreen(helpText1, font, Settings.colorMainText, helpX, helpY, spriteBatch, 100);
                TextWrapping.DrawStringOnScreen(helpText2, font, Settings.colorMainText, helpX, helpY + 40, spriteBatch, 100);
            }

            return textureMedal;
        }

    }
}
