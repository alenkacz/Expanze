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
using System;
using CorePlugin;
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
        readonly int playerSpace = 50;
        List<PlayerSettingRowComponent> playersSettings = new List<PlayerSettingRowComponent>();
        List<MapSettingRowComponent> mapSettings = new List<MapSettingRowComponent>();
        List<ButtonComponent> addButtons = new List<ButtonComponent>();
        List<ButtonComponent> remButtons = new List<ButtonComponent>();

        int activeNumberOfPlayers = 2;
        ButtonComponent startGameButton;

        List<ButtonComponent> playerButtons = new List<ButtonComponent>();

        MapSettingRowComponent points;
        MapSettingRowComponent mapType;
        MapSettingRowComponent mapSize;
        MapSettingRowComponent wealth;


        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public HotSeatScreen()
        {
            colorPosition = new Vector2(150, 100);

            // clearing all players in case of several game in one program launch
            GameMaster.Inst().DeleteAllPlayers();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            GameState.playerNameFont = content.Load<SpriteFont>("playername");

            startGameButton = new ButtonComponent(ScreenManager.Game, (int)(Settings.maximumResolution.X - 150), (int)(Settings.maximumResolution.Y - 200), new Rectangle(), GameState.gameFont, Settings.scaleW(79), Settings.scaleH(66), "HUD/hotseat_hra_button");
            startGameButton.Actions += StartGameSelected;
            //guiComponents.Add(changeTurnButton);

            ButtonComponent backButton = new ButtonComponent(ScreenManager.Game, 60, (int)(Settings.maximumResolution.Y - 200), new Rectangle(), GameState.gameFont, Settings.scaleW(79), Settings.scaleH(66), "HUD/hotseat_back");
            backButton.Actions += BackSelected;
            guiComponents.Add(backButton);

            int counter = 0;

            /*
             * Loading AI 
             */
            List<String> AIname = new List<String>();
            AIname.Add(Strings.MENU_HOT_SEAT_NO_AI);
            foreach (IComponentAI AI in CoreProviderAI.AI)
            {
                AIname.Add(AI.GetAIName());
            }
            Settings.PlayerState = AIname;

            /*
             * 
             */

            foreach (Color c in Settings.playerColors)
            {
                PlayerSettingRowComponent pSwitch = new PlayerSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y, GameState.playerNameFont, 200, 200, c, Strings.MENU_HOT_SEAT_NAMES[counter]);
                playersSettings.Add(pSwitch);
                colorPosition.Y += playerSpace;

                if (counter <= 1)
                {
                    // first two users are active
                    pSwitch.setActive(true);
                }

                counter++;
            }

            //margin betweent sections
            colorPosition.Y += 80;

            points = new MapSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y, GameState.gameFont, 400, 200, Strings.MENU_HOT_SEAT_POINTS, new List<String>() { "50", "75", "100" });
            mapType = new MapSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y + 50, GameState.gameFont, 400, 200, Strings.MENU_HOT_SEAT_MAP_TYPE, new List<String>() { Strings.GAME_SETTINGS_MAP_TYPE_LOWLAND, Strings.GAME_SETTINGS_MAP_TYPE_NORMAL, Strings.GAME_SETTINGS_MAP_TYPE_WASTELAND });
            mapSize = new MapSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y + 100, GameState.gameFont, 400, 200, Strings.MENU_HOT_SEAT_MAP_SIZE, new List<String>() { Strings.GAME_SETTINGS_MAP_SIZE_SMALL, Strings.GAME_SETTINGS_MAP_SIZE_MEDIUM, Strings.GAME_SETTINGS_MAP_SIZE_BIG });
            wealth = new MapSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y + 150, GameState.gameFont, 400, 200, Strings.MENU_HOT_SEAT_MAP_WEALTH, new List<String>() { Strings.GAME_SETTINGS_MAP_WEALTH_LOW, Strings.GAME_SETTINGS_MAP_WEALTH_MEDIUM, Strings.GAME_SETTINGS_MAP_WEALTH_HIGH });

            guiComponents.Add(points);
            guiComponents.Add(mapType);
            guiComponents.Add(mapSize);
            guiComponents.Add(wealth);

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
                startGameButton.Initialize(); startGameButton.LoadContent();
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

            startGameButton.UnloadContent();
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
                p.Update(gameTime);
            }

            startGameButton.Update(gameTime);
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
        /// Event handler for when the Back button is selected
        /// </summary>
        void BackSelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new BackgroundScreen(), e.PlayerIndex);
            ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);

            ScreenManager.RemoveScreen(this);
        }


        private void saveScreenData()
        {
            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                Player pl = p.getPlayerSettings();

                if (pl != null)
                {
                    GameMaster.Inst().AddPlayer(pl);
                }
            }

            int point = Int16.Parse(points.getSelectedSettings());
            String type = mapType.getSelectedSettings();
            String size = mapSize.getSelectedSettings();
            String wealthMap = wealth.getSelectedSettings();

            GameMaster.Inst().ResetGameSettings();
            GameMaster.Inst().setGameSettings(point,type,size,wealthMap);
        }

        #region Draw

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Draw(gameTime);
            }

            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                p.Draw(gameTime);
            }

            //counting all players
            int playerCount = 0;

            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                if (p.isActive())
                {
                    playerCount++;
                }
            }

            if( playerCount >= 2 )
                startGameButton.Draw(gameTime);

            spriteBatch.End();
        }

        #endregion
    }
}

