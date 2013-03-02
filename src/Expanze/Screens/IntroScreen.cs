
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Expanze.Utils.Music;
#endregion

namespace Expanze
{

    class IntroScreen : GameScreen
    {
        #region Fields

        bool loadingIsSlow;
        bool otherScreensAreGone;
        Texture2D backgroundTexture;


        GameScreen[] screensToLoad;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private IntroScreen(ScreenManager screenManager, bool loadingIsSlow,
                              GameScreen[] screensToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
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
            IntroScreen loadingScreen = new IntroScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad); 

            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }

        public override void LoadContent()
        {
            if (ScreenManager.Game.Content == null)
                ScreenManager.Game.Content = new ContentManager(ScreenManager.Game.Services, "Content");
            GameState.game = ScreenManager.Game;
            GameResources.game = ScreenManager.Game;

            GameResources.Inst().LoadMenuTexture();
            backgroundTexture = GameResources.Inst().GetMenuTexture(MenuTexture.LogoBackground);

            MusicManager.Inst().Content = ScreenManager.Game.Content;
            MusicManager.Inst().PlaySong(SongEnum.menu);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {

            checkFinished(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (otherScreensAreGone && checkFinished(gameTime))
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen, ControllingPlayer);
                    }
                }

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        /// <summary>
        /// Checks whether the intro screen was already cancelled
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool checkFinished(GameTime time)
        {
            bool keyPress = false;

            try
            {
                keyPress = Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Escape);
            }
            catch
            {
            }
            if (time.TotalGameTime.TotalSeconds > 4.5 || keyPress || Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                InputState.waitForRelease();
                return true;
            }

            return false;
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (loadingIsSlow)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

                // Center the text in the viewport.
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

                Color color = Color.White * TransitionAlpha;

                spriteBatch.Begin();

                spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

                spriteBatch.End();
            }
        }


        #endregion
    }
}
