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
using System.Threading;
using Expanze.Utils;
#endregion

namespace Expanze
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class GameLoadScreen : GameScreen
    {
        #region Fields

        bool gameResourcesLoaded = false;
        bool otherScreensAreGone;
        Thread loadingThread;

        GameScreen[] screensToLoad;
        string[] tips;
        string tip;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private GameLoadScreen(ScreenManager screenManager,
                              GameScreen[] screensToLoad)
        {
            this.screensToLoad = screensToLoad;
            otherScreensAreGone = false;

            tips = new string[11];
            tips[0] = Strings.Inst().GetString(TextEnum.TIP_GAME_1);
            tips[1] = Strings.Inst().GetString(TextEnum.TIP_GAME_2);
            tips[2] = Strings.Inst().GetString(TextEnum.TIP_GAME_3);
            tips[3] = Strings.Inst().GetString(TextEnum.TIP_GAME_4);
            tips[4] = Strings.Inst().GetString(TextEnum.TIP_GAME_5);
            tips[5] = Strings.Inst().GetString(TextEnum.TIP_GAME_6);
            tips[6] = Strings.Inst().GetString(TextEnum.TIP_GAME_7);
            tips[7] = Strings.Inst().GetString(TextEnum.TIP_GAME_8);
            tips[8] = Strings.Inst().GetString(TextEnum.TIP_GAME_9);
            tips[9] = Strings.Inst().GetString(TextEnum.TIP_GAME_10);
            tips[10] = Strings.Inst().GetString(TextEnum.TIP_GAME_11);
            tip = tips[GameMaster.Inst().GetRandomInt(tips.Length)];
        }


        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                PlayerIndex? controllingPlayer,
                                params GameScreen[] screensToLoad)
        {
            bool oneBackground = false;
            foreach (GameScreen screen in screenManager.GetScreens())
            {
                if(screen is MainMenuScreen)
                    screen.ExitScreen();
                if (screen is BackgroundScreen && !oneBackground)
                    oneBackground = true;
                else if (screen is BackgroundScreen && oneBackground)
                    screen.ExitScreen();
            }

            GameResources.game = screenManager.Game;
            //screenManager.AddScreen();
            screenManager.AddScreen(new GameLoadScreen(screenManager, screensToLoad), controllingPlayer);
        }


        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);


            if (!otherScreensAreGone &&
                (ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length <= 2))
            {
                otherScreensAreGone = true;
                loadingThread = new Thread(X => GameResources.Inst().LoadContent());
                loadingThread.Start();
            }

            if (otherScreensAreGone)
            {
                if(!loadingThread.IsAlive)
                    gameResourcesLoaded = true;

                if (gameResourcesLoaded)
                {
                    foreach (GameScreen screen in screensToLoad)
                    {
                        if (screen != null)
                        {
                            ScreenManager.AddScreen(screen, ControllingPlayer);
                        }
                    }
                    ScreenManager.Game.ResetElapsedTime();
                    ScreenManager.RemoveScreen(this);
                }
            }
        }


        //int time = 0;
        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {


            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            SpriteFont fontTip = GameResources.Inst().GetFont(EFont.MedievalMedium);
            
            // Center the text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            String message1 = Strings.Inst().GetString(TextEnum.MENU_GAME_LOADING_TITLE);
            Vector2 textSize1 = font.MeasureString(message1);
            Vector2 textPosition1 = (viewportSize - textSize1) / 2;
            textPosition1.Y -= textSize1.Y / 2.0f;
            String message2 = GameResources.Inst().GetProgress();
            Vector2 textSize2 = font.MeasureString(message2);         
            Vector2 textPosition2 = (viewportSize - textSize2) / 2;
            textPosition2.Y += textSize2.Y / 2.0f;

           
            Color color = Color.BurlyWood * TransitionAlpha;

            // Draw the text.
            spriteBatch.Begin();
            spriteBatch.DrawString(font, message1, textPosition1, color);
            spriteBatch.DrawString(font, message2, textPosition2, color);
            if (fontTip != null)
            {
                Vector2 tipPosition = (viewportSize - fontTip.MeasureString(tip)) / 2;
                tipPosition.Y += font.LineSpacing + fontTip.LineSpacing;

                if (tipPosition.X > 80)
                    spriteBatch.DrawString(fontTip, tip, tipPosition, color);
                else
                    TextWrapping.DrawStringIntoRectangle(tip, fontTip, color, 80, tipPosition.Y, 600);
            }
            spriteBatch.End();

        }


        #endregion
    }
}
