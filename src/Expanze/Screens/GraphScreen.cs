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
        int graphKind;

        GameScreen[] screensToLoad;
        ScreenManager screenManager;
        PrimitiveBatch primitiveBatch;

        private const float border = 120.0f;
        private float windowWidth;
        private float windowHeight;
        int columnNumber;
        int rowNumber;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private GraphScreen(ScreenManager screenManager,
                              GameScreen[] screensToLoad)
        {
            graphKind = 0;
            this.userCancelled = false;
            this.screensToLoad = screensToLoad;
            this.screenManager = screenManager;
            this.primitiveBatch = new PrimitiveBatch(screenManager.GraphicsDevice);
            windowWidth = screenManager.GraphicsDevice.Viewport.Width;
            windowHeight = screenManager.GraphicsDevice.Viewport.Height;
            columnNumber = GameMaster.Inst().GetTurnNumber();
            SetRowNumber();
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
            {
                if(!(screen is BackgroundScreen))
                     screen.ExitScreen();
            }

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

            if (input.IsNewKeyPress(Keys.Left, null, out index))
            {
                graphKind--;
                if (graphKind < 0)
                    graphKind = (int)Statistic.Kind.Count - 1;
                SetRowNumber();
            } else if(input.IsNewKeyPress(Keys.Right, null, out index))
            {
                graphKind = (++graphKind) % (int) Statistic.Kind.Count;
                SetRowNumber();
            }

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
            SpriteFont fontBig;
            SpriteFont fontSmall;

            String message = "Graf";

            // Center the text in the viewport.
            fontBig = GameResources.Inst().GetFont(EFont.MedievalBig);
            fontSmall = GameResources.Inst().GetFont(EFont.MedievalSmall);
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = fontBig.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 6;

            Color color = Color.White * TransitionAlpha;

            //textPosition.Y -= 150;
            int startY = (int)textPosition.Y + 60;

            DrawGraph((Statistic.Kind)graphKind);

            // Draw the text.
            spriteBatch.Begin();
            spriteBatch.DrawString(fontBig, message, textPosition, Color.Black);
            spriteBatch.DrawString(fontBig, message, textPosition + new Vector2(2, -2), color);
            textPosition += new Vector2(40, 40);
            spriteBatch.DrawString(fontSmall, Statistic.GetGraphName((Statistic.Kind)graphKind), textPosition, Color.Black);
            spriteBatch.DrawString(fontSmall, Statistic.GetGraphName((Statistic.Kind)graphKind), textPosition + new Vector2(2,-2), color);
            spriteBatch.End();        
        }

        private void DrawGraph(Statistic.Kind kind)
        {
            List<Player> players = GameMaster.Inst().GetPlayers();

            Color c;
            int[][] statistic;
            int sum;
            int k = (int)kind;
           
            int playerID = 0;
            float offset = 1.0f;

            foreach (Player player in players)
            {
                c = player.GetColor();
                sum = 0;
                statistic = player.GetStatistic().GetStat();
                primitiveBatch.Begin(PrimitiveType.LineStrip);
                primitiveBatch.AddVertex(new Vector2(playerID * offset + border + 0 * (windowWidth - border * 2) / (float)columnNumber, 
                                                     windowHeight - border - sum * (windowHeight - border * 2) / (float)rowNumber), c);

                for (int loop1 = 0; loop1 <= columnNumber; loop1++)
                {
                    if (k == (int)Statistic.Kind.SumSources)
                        sum = statistic[k][loop1];
                    else
                        sum += statistic[k][loop1];

                    float xbase = border + loop1 * (windowWidth - border * 2) / (float)columnNumber;
                    float ybase = windowHeight - border;
                    primitiveBatch.AddVertex(new Vector2(playerID * offset + xbase,
                                                         ybase - sum * (windowHeight - border * 2) / (float)rowNumber + 1), c);                  
                }

                primitiveBatch.End();
                playerID++;
            }

            DrawAxisNumbers();
        }

        private void DrawAxisNumbers()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = GameResources.Inst().GetFont(EFont.HudMaterialsFont);

            spriteBatch.Begin();
            for (int loop1 = 0; loop1 < columnNumber + 1; loop1 += 5)
            {
                string number = loop1 + "";
                Vector2 d = font.MeasureString(number);

                float xbase = border + loop1 * (windowWidth - border * 2) / (float)columnNumber;
                float ybase = windowHeight - border;

                spriteBatch.DrawString(font, number, new Vector2(xbase - d.X / 2.0f, ybase + d.Y / 3.0f), Color.White);
            }

            for (int loop1 = 0; loop1 < rowNumber + 1; loop1 += rowNumber / 5 + 1)
            {
                float xbase = windowWidth - border;
                float ybase = windowHeight - border  - loop1 * (windowHeight - border * 2) / (float)rowNumber;
                string number = loop1 + "";
                float dy = font.MeasureString(number).Y;
                spriteBatch.DrawString(font, number, new Vector2(xbase, ybase - dy / 2), Color.White);
            }
            spriteBatch.End();
        }
        #endregion

        private void SetRowNumber()
        {
            int k = this.graphKind;
            int sum;
            int[][] statistic;

            rowNumber = 2;
            foreach (Player player in GameMaster.Inst().GetPlayers())
            {
                sum = 0;
                statistic = player.GetStatistic().GetStat();
                for (int loop1 = 0; loop1 <= columnNumber; loop1++)
                {
                    if (k == (int)Statistic.Kind.SumSources)
                    {
                        if (statistic[k][loop1] > rowNumber)
                            rowNumber = statistic[k][loop1];
                    }
                    else
                    {
                        sum += statistic[k][loop1];

                        if (sum > rowNumber)
                            rowNumber = sum;
                    }
                }
            }
        }
    }
}
