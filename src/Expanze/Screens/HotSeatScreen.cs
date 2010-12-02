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
    class HotSeatScreen : GameScreen
    {
        List<GuiComponent> guiComponents = new List<GuiComponent>();
        ContentManager content;
        Texture2D playerColorTexture;
        Vector2 colorPosition;
        //space between player rows
        readonly int playerSpace = 80;
        List<PlayerSettingRowComponent> playersSettings = new List<PlayerSettingRowComponent>();

        List<ButtonComponent> playerButtons = new List<ButtonComponent>();

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        private HotSeatScreen(bool isAI)
        {
            colorPosition = new Vector2(50, 100);
        }

        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager,
                                PlayerIndex? controllingPlayer)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            HotSeatScreen loadingScreen = new HotSeatScreen(false);

            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            GameState.playerNameFont = content.Load<SpriteFont>("playername");

            ButtonComponent changeTurnButton = new ButtonComponent(ScreenManager.Game, (int)(Settings.maximumResolution.X - 167), (int)(Settings.maximumResolution.Y - 161), new Rectangle(), GameState.gameFont, Settings.scaleW(147), Settings.scaleH(141), "nextTurn");
            changeTurnButton.Actions += StartGameSelected;
            guiComponents.Add(changeTurnButton);

            int counter = 0;

            foreach (Color c in Settings.playerColors)
            {
                PlayerSettingRowComponent pSwitch = new PlayerSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y, GameState.playerNameFont, 200, 200, c, "Player" + (counter+1));
                guiComponents.Add(pSwitch);
                playersSettings.Add(pSwitch);
                colorPosition.Y += playerSpace;

                if (counter > 1)
                {
                    //only two players are active by default
                    pSwitch.setIndexOfText(Settings.PlayerState.IndexOf("Neaktivní"));
                }

                counter++;
            }

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Initialize();
                guiComponent.LoadContent();
            }

            playerColorTexture = ScreenManager.Game.Content.Load<Texture2D>("pcolor");

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
            saveScreenData();

            //ScreenManager.RemoveScreen(this);
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(false));
            ScreenManager.RemoveScreen(this);
        }

        private void saveScreenData()
        {
            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                Player pl = p.getPlayerSettings();

                if (pl != null)
                {
                    GameMaster.getInstance().addPlayer(pl);
                }
            }
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
                                              Color.Black, 0, 0);

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Draw(gameTime);
            }

            spriteBatch.End();
        }

        #endregion
    }
}

