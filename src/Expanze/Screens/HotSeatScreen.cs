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
        List<ButtonComponent> addButtons = new List<ButtonComponent>();
        List<ButtonComponent> remButtons = new List<ButtonComponent>();

        int activeNumberOfPlayers = 2;

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

            ButtonComponent addBtn1 = new ButtonComponent(ScreenManager.Game, (int)colorPosition.X, (int)(colorPosition.Y + activeNumberOfPlayers*playerSpace), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            addBtn1.Actions += AddPlayerSelected;
            addButtons.Add(addBtn1);
            ButtonComponent addBtn2 = new ButtonComponent(ScreenManager.Game, (int)colorPosition.X, (int)(colorPosition.Y + (activeNumberOfPlayers+1) * playerSpace), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            addBtn2.Actions += AddPlayerSelected;
            addButtons.Add(addBtn2);
            ButtonComponent addBtn3 = new ButtonComponent(ScreenManager.Game, (int)colorPosition.X, (int)(colorPosition.Y + (activeNumberOfPlayers + 2) * playerSpace), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            addBtn3.Actions += AddPlayerSelected;
            addButtons.Add(addBtn3);
            ButtonComponent addBtn4 = new ButtonComponent(ScreenManager.Game, (int)colorPosition.X, (int)(colorPosition.Y + (activeNumberOfPlayers + 3) * playerSpace), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            addBtn4.Actions += AddPlayerSelected;
            addButtons.Add(addBtn4);

            ButtonComponent remBtn1 = new ButtonComponent(ScreenManager.Game, (int)colorPosition.X + 104, (int)(colorPosition.Y + activeNumberOfPlayers * playerSpace), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            remBtn1.Actions += RemovePlayerSelected;
            remButtons.Add(remBtn1);
            ButtonComponent remBtn2 = new ButtonComponent(ScreenManager.Game, (int)colorPosition.X + 104, (int)(colorPosition.Y + (activeNumberOfPlayers + 1) * playerSpace), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            remBtn2.Actions += RemovePlayerSelected;
            remButtons.Add(remBtn2);
            ButtonComponent remBtn3 = new ButtonComponent(ScreenManager.Game, (int)colorPosition.X + 104, (int)(colorPosition.Y + (activeNumberOfPlayers + 2) * playerSpace), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            remBtn3.Actions += RemovePlayerSelected;
            remButtons.Add(remBtn3);
            ButtonComponent remBtn4 = new ButtonComponent(ScreenManager.Game, (int)colorPosition.X + 104, (int)(colorPosition.Y + (activeNumberOfPlayers + 3) * playerSpace), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            remBtn4.Actions += RemovePlayerSelected;
            remButtons.Add(remBtn4);

            int counter = 0;

            foreach (Color c in Settings.playerColors)
            {
                PlayerSettingRowComponent pSwitch = new PlayerSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y, GameState.playerNameFont, 200, 200, c, "Player" + (counter+1));
                playersSettings.Add(pSwitch);
                colorPosition.Y += playerSpace;

                if (counter > 1)
                {
                    pSwitch.setIndexOfText(Settings.PlayerState.IndexOf("Neaktivní"));
                }

                counter++;
            }

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Initialize();
                guiComponent.LoadContent();
            }

            foreach( PlayerSettingRowComponent p in playersSettings )
            {
                p.Initialize();
                p.LoadContent();
            }

            foreach (ButtonComponent p in addButtons)
            {
                p.Initialize();
                p.LoadContent();
            }

            foreach (ButtonComponent p in remButtons)
            {
                p.Initialize();
                p.LoadContent();
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

            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                p.UnloadContent();
            }

            foreach (ButtonComponent b in addButtons)
            {
                b.UnloadContent();
            }

            foreach (ButtonComponent b in remButtons)
            {
                b.UnloadContent();
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

            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                p.Initialize();
            }

            foreach (ButtonComponent b in addButtons)
            {
                b.Update(gameTime);
            }

            foreach (ButtonComponent b in remButtons)
            {
                b.Update(gameTime);
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

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void AddPlayerSelected(object sender, PlayerIndexEventArgs e)
        {
            ++activeNumberOfPlayers;
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void RemovePlayerSelected(object sender, PlayerIndexEventArgs e)
        {
            --activeNumberOfPlayers;
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

            for (int i = 0; i < activeNumberOfPlayers; i++)
            {
                playersSettings.ToArray()[i].Draw(gameTime);
            }

            if (activeNumberOfPlayers < 6)
            {
                int count = activeNumberOfPlayers - 2;
                addButtons.ToArray()[count].Draw(gameTime);
                if( activeNumberOfPlayers > 2 )
                    remButtons.ToArray()[count].Draw(gameTime);
            }

            spriteBatch.End();
        }

        #endregion
    }
}

