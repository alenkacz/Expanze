#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using CorePlugin;
#endregion

namespace Expanze
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class VictoryScreen : GameScreen
    {
        #region Fields

        bool userCancelled;

        bool firstTimeHandleInput;
        bool wasMousePressedWhenVictory;
        ButtonComponent toMenuButton;
        ButtonComponent toGraphButton;

        ScreenManager screenManager;

        GameScreen[] screensToLoad;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private VictoryScreen(ScreenManager screenManager,
                              GameScreen[] screensToLoad)
        {
            this.userCancelled = false;
            firstTimeHandleInput = true;
            this.screensToLoad = screensToLoad;
            this.screenManager = screenManager;

            toGraphButton = new ButtonComponent(screenManager.Game, (int)(Settings.maximumResolution.X - 150), (int)(Settings.maximumResolution.Y - 200), new Rectangle(), GameResources.Inst().GetFont(EFont.MedievalBig), Settings.scaleW(79), Settings.scaleH(66), "HUD/menu_next");
            toGraphButton.Actions += ToGraph;

            toMenuButton = new ButtonComponent(screenManager.Game, 60, (int)(Settings.maximumResolution.Y - 200), new Rectangle(), GameResources.Inst().GetFont(EFont.MedievalBig), Settings.scaleW(79), Settings.scaleH(66), "HUD/hotseat_back");
            toMenuButton.Actions += ToMenu;

            toGraphButton.LoadContent();
            toMenuButton.LoadContent();
        }


        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                PlayerIndex? controllingPlayer,
                                params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            VictoryScreen loadingScreen = new VictoryScreen(screenManager,
                                                            screensToLoad);

            screenManager.AddScreen(new BackgroundScreen(), controllingPlayer);
            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }


        #endregion

        #region Update and Draw

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            MouseState mouseState = input.CurrentMouseState;

            if (firstTimeHandleInput)
            {
                wasMousePressedWhenVictory = mouseState.LeftButton == ButtonState.Pressed;
                firstTimeHandleInput = false;
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Released)
                    wasMousePressedWhenVictory = false;

                PlayerIndex index;

                if (input.IsNewKeyPress(Keys.Tab, null, out index))
                {
                    AddGraphScreen();
                }

                if (input.IsNewKeyPress(Keys.Escape, null, out index) || 
                    input.IsNewKeyPress(Keys.Enter, null, out index))
                {
                    InputState.waitForRelease();
                    userCancelled = true;
                }
            }
        }

        private void AddGraphScreen()
        {
            toMenuButton.Disabled = true;
            toGraphButton.Disabled = true;
            GraphScreen.Load(screenManager, false, ControllingPlayer, new GameScreen[] { this });
        }

        /// <summary>
        /// Event handler for when the Back button is selected
        /// </summary>
        void ToMenu(object sender, PlayerIndexEventArgs e)
        {
            userCancelled = true;
        }

        /// <summary>
        /// Event handler for when the Back button is selected
        /// </summary>
        void ToGraph(object sender, PlayerIndexEventArgs e)
        {
            AddGraphScreen();
        }


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            toGraphButton.Update(gameTime);
            toMenuButton.Update(gameTime);
            if (toMenuButton.Disabled)
            {
                toMenuButton.Disabled = false;
                toGraphButton.Disabled = false;
            }
            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (userCancelled)
            {
                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen, ControllingPlayer);
                    }
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
                ScreenManager.RemoveScreen(this);
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {


                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font;

                String message = Strings.Inst().GetString(TextEnum.MENU_VICTORY_SCREEN_STATISTIC_AFTER) + (GameMaster.Inst().GetTurnNumber() - 1) + Strings.Inst().GetString(TextEnum.MENU_VICTORY_SCREEN_TURN);

                // Center the text in the viewport.
                font = GameResources.Inst().GetFont(EFont.MenuFont);
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = new Vector2((Settings.maximumResolution.X - textSize.X) / 2.0f, 300);

                Color color = Settings.colorMenuText * TransitionAlpha;

                textPosition.Y -= 180;
                int startY = (int) textPosition.Y + 70;
                int startX = 140;
                
                // Draw the text.
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
                spriteBatch.DrawString(font, message, textPosition, color);
                font = GameResources.Inst().GetFont(EFont.MedievalBig);

                Player[] players = new Player[GameMaster.Inst().GetPlayers().Count];
                GameMaster.Inst().GetPlayers().CopyTo(players);

                Array.Sort(players, delegate(Player p1, Player p2)
                {
                    return p1.GetPointSum().CompareTo(p2.GetPointSum()); // (user1.Age - user2.Age)
                });
                Array.Reverse(players);

                int startName = startX + 80; // 150 + 80 * DrawAllPoints(players);
                DrawAllPoints(players);
                startY += 123;
                foreach(Player player in players) {
                    spriteBatch.Draw(GameResources.Inst().GetHudTexture(HUDTexture.PlayerColor), new Vector2(startX + 20, startY), player.GetColor());
                    spriteBatch.DrawString(font, player.GetName(), new Vector2(startName, startY), color);
                    //if(player.GetIsAI())
                    //    spriteBatch.DrawString(font, player.GetComponentAI().GetAIName(), new Vector2(startX + 450, startY), color);
                    //spriteBatch.DrawString(font, player.GetPointSum() + "", new Vector2(startX + 80, startY), color);
                    startY += 50;
                }

                
                spriteBatch.End();

                //toGraphButton.Draw(gameTime);
                toMenuButton.Draw(gameTime);
        }


        #endregion

        private int DrawAllPoints(Player[] players)
        {
            int pointKind = 0;

            if (Settings.pointsTown > 0)
                pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_TOWN_PART1), Strings.Inst().GetString(TextEnum.GOAL_TOWN_PART2), GameMaster.Inst().GetMedaileIcon(Building.Town), PlayerPoints.Town, Settings.pointsTown, pointKind);

            if (Settings.pointsRoad > 0)
                pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_ROAD_PART1), Strings.Inst().GetString(TextEnum.GOAL_ROAD_PART2), GameMaster.Inst().GetMedaileIcon(Building.Road), PlayerPoints.Road, Settings.pointsRoad, pointKind);

            if (Settings.goalRoadID > 0)
                pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_ROADID_PART1), Strings.Inst().GetString(TextEnum.GOAL_ROADID_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconMedalRoadID), PlayerPoints.RoadID, Settings.goalRoadID, pointKind);
            if (Settings.goalTownID > 0)
                pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_TOWNID_PART1), Strings.Inst().GetString(TextEnum.GOAL_TOWNID_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconMedalTownID), PlayerPoints.TownID, Settings.goalTownID, pointKind);

            if (Settings.pointsMill > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_MILL_PART1), Strings.Inst().GetString(TextEnum.GOAL_MILL_PART2), GameMaster.Inst().GetMedaileIcon(Building.Mill), PlayerPoints.Mill, Settings.pointsMill, pointKind);
            if (Settings.pointsMine > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_MINE_PART1), Strings.Inst().GetString(TextEnum.GOAL_MINE_PART2), GameMaster.Inst().GetMedaileIcon(Building.Mine), PlayerPoints.Mine, Settings.pointsMine, pointKind);
            if (Settings.pointsQuarry > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_QUARRY_PART1), Strings.Inst().GetString(TextEnum.GOAL_QUARRY_PART2), GameMaster.Inst().GetMedaileIcon(Building.Quarry), PlayerPoints.Quarry, Settings.pointsQuarry, pointKind);
            if (Settings.pointsSaw > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_SAW_PART1), Strings.Inst().GetString(TextEnum.GOAL_SAW_PART2), GameMaster.Inst().GetMedaileIcon(Building.Saw), PlayerPoints.Saw, Settings.pointsSaw, pointKind);
            if (Settings.pointsStepherd > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_STEPHERD_PART1), Strings.Inst().GetString(TextEnum.GOAL_STEPHERD_PART2), GameMaster.Inst().GetMedaileIcon(Building.Stepherd), PlayerPoints.Stepherd, Settings.pointsStepherd, pointKind);
            if (Settings.pointsFort > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_FORT_PART1), Strings.Inst().GetString(TextEnum.GOAL_FORT_PART2), GameMaster.Inst().GetMedaileIcon(Building.Fort), PlayerPoints.Fort, Settings.pointsFort, pointKind);
            if (Settings.pointsMarket > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_MARKET_PART1), Strings.Inst().GetString(TextEnum.GOAL_MARKET_PART2), GameMaster.Inst().GetMedaileIcon(Building.Market), PlayerPoints.Market, Settings.pointsMarket, pointKind);
            if (Settings.pointsMonastery > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_MONASTERY_PART1), Strings.Inst().GetString(TextEnum.GOAL_MONASTERY_PART2), GameMaster.Inst().GetMedaileIcon(Building.Monastery), PlayerPoints.Monastery, Settings.pointsMonastery, pointKind);
            if (Settings.pointsFortParade > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_FORT_PARADE_PART1), Strings.Inst().GetString(TextEnum.GOAL_FORT_PARADE_PART2), Settings.Game.Content.Load<Texture2D>("HUD/score_medaile"), PlayerPoints.FortParade, Settings.pointsFortParade, pointKind);
            if (Settings.pointsFortCapture > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_FORT_CAPTURE_PART1), Strings.Inst().GetString(TextEnum.GOAL_FORT_CAPTURE_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconFortCapture), PlayerPoints.FortCaptureHexa, Settings.pointsFortCapture, pointKind);
            if (Settings.pointsFortSteal > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_FORT_STEAL_PART1), Strings.Inst().GetString(TextEnum.GOAL_FORT_STEAL_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconFortSources), PlayerPoints.FortStealSources, Settings.pointsFortSteal, pointKind);
            if (Settings.pointsMarketLvl1 > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_LICENCE1_PART1), Strings.Inst().GetString(TextEnum.GOAL_LICENCE1_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconOre1), PlayerPoints.LicenceLvl1, Settings.pointsMarketLvl1, pointKind);
            if (Settings.pointsUpgradeLvl1 > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_UPGRADE1_PART1), Strings.Inst().GetString(TextEnum.GOAL_UPGRADE1_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry1), PlayerPoints.UpgradeLvl1, Settings.pointsUpgradeLvl1, pointKind);
            if (Settings.pointsMarketLvl2 > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_LICENCE2_PART1), Strings.Inst().GetString(TextEnum.GOAL_LICENCE2_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconOre2), PlayerPoints.LicenceLvl2, Settings.pointsMarketLvl2, pointKind);
            if (Settings.pointsUpgradeLvl2 > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_UPGRADE2_PART1), Strings.Inst().GetString(TextEnum.GOAL_UPGRADE2_PART2), GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry2), PlayerPoints.UpgradeLvl2, Settings.pointsUpgradeLvl2, pointKind);
            if (Settings.pointsCorn > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_CORN_PART1), Strings.Inst().GetString(TextEnum.GOAL_CORN_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallCorn), PlayerPoints.Corn, Settings.pointsCorn, pointKind);
            if (Settings.pointsMeat > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_MEAT_PART1), Strings.Inst().GetString(TextEnum.GOAL_MEAT_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallMeat), PlayerPoints.Meat, Settings.pointsMeat, pointKind);
            if (Settings.pointsStone > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_STONE_PART1), Strings.Inst().GetString(TextEnum.GOAL_STONE_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallStone), PlayerPoints.Stone, Settings.pointsStone, pointKind);
            if (Settings.pointsWood > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_WOOD_PART1), Strings.Inst().GetString(TextEnum.GOAL_WOOD_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallWood), PlayerPoints.Wood, Settings.pointsWood, pointKind);
            if (Settings.pointsOre > 0) pointKind = DrawPoints(players, Strings.Inst().GetString(TextEnum.GOAL_ORE_PART1), Strings.Inst().GetString(TextEnum.GOAL_ORE_PART2), GameResources.Inst().GetHudTexture(HUDTexture.SmallOre), PlayerPoints.Ore, Settings.pointsOre, pointKind);

            return pointKind;
        }

        private int DrawPoints(Player[] players, string text1, string text2, Texture2D texture2D, PlayerPoints pointKind, int goal, int pointNumber)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            int x = 500 + pointNumber * 80;
            int y = 300;
            spriteBatch.Draw(texture2D, new Vector2(x, y - texture2D.Height), Color.White);

            SpriteFont font = GameResources.Inst().GetFont(EFont.MedievalBig);
            int count = 0;
            foreach (Player player in players)
            {
                int points = player.GetPoints()[(int)pointKind];
                switch (pointKind)
                {
                    case PlayerPoints.Corn: points = player.GetCollectSourcesNormal().GetCorn(); break;
                    case PlayerPoints.Meat: points = player.GetCollectSourcesNormal().GetMeat(); break;
                    case PlayerPoints.Stone: points = player.GetCollectSourcesNormal().GetStone(); break;
                    case PlayerPoints.Wood: points = player.GetCollectSourcesNormal().GetWood(); break;
                    case PlayerPoints.Ore: points = player.GetCollectSourcesNormal().GetOre(); break;
                    default: points = player.GetPoints()[(int)pointKind]; break;
                }

                int showPoints = points;
                if (showPoints > goal)
                    showPoints = goal;

                String pointsString = "" + showPoints;

                spriteBatch.DrawString(font, pointsString, new Vector2(x + 25 - font.MeasureString(pointsString).X / 2, y + 50 * count + 12), Settings.colorMenuText);
                count++;
            }

            return pointNumber + 1;
        }
    }
}
