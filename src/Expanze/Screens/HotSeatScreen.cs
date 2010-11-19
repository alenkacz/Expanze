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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Expanze
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class HotSeatScreen : MenuScreen
    {
        List<GuiComponent> guiComponents = new List<GuiComponent>();
        ContentManager content;

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public HotSeatScreen()
            :base("Test")
        {

        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");


            ButtonComponent changeTurnButton = new ButtonComponent(ScreenManager.Game, ScreenManager.Game.GraphicsDevice.Viewport.Width - 91, ScreenManager.Game.GraphicsDevice.Viewport.Height - 80, new Rectangle(), GameState.gameFont, 91, 80, "nextTurn");
            changeTurnButton.Actions += StartGameSelected;
            guiComponents.Add(changeTurnButton);

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Initialize();
                guiComponent.LoadContent();
            }

            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.UnloadContent();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Update(gameTime);
            }
        }


        #endregion

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void StartGameSelected(object sender, PlayerIndexEventArgs e)
        {
            //ScreenManager.RemoveScreen(this);
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(false));
            //ScreenManager.AddScreen(new GameplayScreen(false),PlayerIndex.One);
        }

        #region Draw

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                              Color.White, 0, 0);

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Draw(gameTime);
            }

            spriteBatch.End();
        }

        #endregion
    }
}

