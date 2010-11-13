#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Expanze
{
   
    public class ExpanzeGame : Microsoft.Xna.Framework.Game
    {

        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        CustomCursor cursorComp;


        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "gradient",
        };

        
        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public ExpanzeGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            Settings.GraphicsDeviceManager = graphics;
            Settings.Game = this;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            cursorComp = new CustomCursor(this);
            Components.Add(cursorComp);

            // Activate the first screens.
            GameScreen[] gs = { new BackgroundScreen(), new MainMenuScreen() };
            IntroScreen.Load(screenManager, true, null, gs);
        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (ExpanzeGame game = new ExpanzeGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
