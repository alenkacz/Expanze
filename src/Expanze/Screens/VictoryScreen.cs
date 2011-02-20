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

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            MouseState mouseState = input.CurrentMouseState;

            if (keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Enter) || (Mouse.GetState().LeftButton == ButtonState.Pressed && !wasMousePressedWhenVictory))
            {
                InputState.waitForRelease();
                userCancelled = true;
            }

            if (firstTimeHandleInput)
            {
                wasMousePressedWhenVictory = Mouse.GetState().LeftButton == ButtonState.Pressed;
                firstTimeHandleInput = false;
            }
            else
            {
                if (Mouse.GetState().LeftButton == ButtonState.Released)
                    wasMousePressedWhenVictory = false;
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

                String message = "Vítìzství! A prohra jiného v " + GameMaster.Inst().GetTurnNumber() +". kole.";

                // Center the text in the viewport.
                font = GameResources.Inst().GetFont(EFont.MedievalBig);
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * TransitionAlpha;

                textPosition.Y -= 150;
                int startY = (int) textPosition.Y + 70;
                int startX = 100;
                
                // Draw the text.
                spriteBatch.Begin();
                spriteBatch.DrawString(font, message, textPosition, color);
                font = GameResources.Inst().GetFont(EFont.MedievalMedium);
                foreach(Player player in GameMaster.Inst().GetPlayers()) {
                    spriteBatch.Draw(GameResources.Inst().GetHudTexture(HUDTexture.PlayerColor), new Vector2(startX + 20, startY), player.GetColor());
                    spriteBatch.DrawString(font, player.GetName(), new Vector2(startX + 140, startY), color);
                    if(player.GetIsAI())
                        spriteBatch.DrawString(font, player.GetComponentAI().GetAIName(), new Vector2(startX + 450, startY), color);
                    spriteBatch.DrawString(font, player.GetPoints() + "", new Vector2(startX + 80, startY), color);
                    startY += 50;
                }
                spriteBatch.End();
            
        }


        #endregion
    }
}
