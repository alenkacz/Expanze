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
using Expanze.Utils;
using Expanze.Gameplay;
#endregion

namespace Expanze
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class GraphScreen : GameScreen
    {
        #region Fields

        bool userCancelled;

        GameScreen[] screensToLoad;
        ScreenManager screenManager;
        PrimitiveBatch primitiveBatch;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private GraphScreen(ScreenManager screenManager,
                              GameScreen[] screensToLoad)
        {
            this.userCancelled = false;
            this.screensToLoad = screensToLoad;
            this.screenManager = screenManager;
            this.primitiveBatch = new PrimitiveBatch(screenManager.GraphicsDevice);
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
            GraphScreen loadingScreen = new GraphScreen(screenManager,
                                                            screensToLoad);

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

            PlayerIndex index;

            if (input.IsNewKeyPress(Keys.Tab, null, out index) || 
                input.IsNewKeyPress(Keys.Enter, null, out index) ||
                input.IsNewKeyPress(Keys.Escape, null, out index) ||
                input.IsNewLeftMouseButtonPressed())
            {
                InputState.waitForRelease();
                userCancelled = true;
            }
        }


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

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
                InputState.waitForRelease();
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

            String message = "Grafy";

            // Center the text in the viewport.
            font = GameResources.Inst().GetFont(EFont.MedievalBig);
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            Color color = Color.White * TransitionAlpha;

            textPosition.Y -= 150;
            int startY = (int)textPosition.Y + 70;
            int startX = 100;

            // Draw the text.
            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, textPosition, color);
            spriteBatch.End();

            DrawGraph(Statistic.Kind.Points);
        }

        private void DrawGraph(Statistic.Kind kind)
        {
            List<Player> players = GameMaster.Inst().GetPlayers();

            Color c;
            int[][] statistic;
            int col = GameMaster.Inst().GetTurnNumber();
            int sum;
            int k = (int) kind;

            float windowWidth = screenManager.GraphicsDevice.Viewport.Width;
            float windowHeight = screenManager.GraphicsDevice.Viewport.Height;

            int row = 0;
            foreach (Player player in players)
            {
                sum = 0;
                statistic = player.GetStatistic().GetStat();
                for (int loop1 = 0; loop1 < col; loop1++)
                {
                    sum += statistic[k][loop1];
                }
                if (sum > row)
                    row = sum;
            }
            foreach (Player player in players)
            {
                c = player.GetColor();
                sum = 0;
                statistic = player.GetStatistic().GetStat();
                primitiveBatch.Begin(PrimitiveType.LineStrip);
                for (int loop1 = 0; loop1 < col; loop1++)
                {
                    sum += statistic[k][loop1];
                    primitiveBatch.AddVertex(new Vector2(30.0f + loop1 * (windowWidth - 60.0f) / (float)col, windowHeight - 30.0f - sum * (windowHeight - 60.0f) / (float)row), c);
                }
                primitiveBatch.End();
            }
        }

        #endregion
    }
}
