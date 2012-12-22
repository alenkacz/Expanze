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
using Expanze.Utils.Music;
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

        //int activeNumberOfPlayers = 2;
        ButtonComponent startGameButton;

        List<ButtonComponent> playerButtons = new List<ButtonComponent>();

        MapSettingRowComponent mapType;
        MapSettingRowComponent sourceKind;
        MapSettingRowComponent secretProductivity;
        MapSettingRowComponent secretKind;

        Random random;

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public HotSeatScreen()
        {
            colorPosition = new Vector2(150, 100);
            random = new Random();
            // clearing all players in case of several game in one program launch
            GameMaster.Inst().DeleteAllPlayers();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }
            else
            {
                GameMaster.Inst().DeleteAllPlayers();
                return;
            }

            startGameButton = new ButtonComponent(ScreenManager.Game, (int)(Settings.maximumResolution.X - 150), (int)(Settings.maximumResolution.Y - 200), new Rectangle(), GameResources.Inst().GetFont(EFont.MedievalBig), Settings.scaleW(79), Settings.scaleH(66), "HUD/hotseat_hra_button");
            startGameButton.Actions += StartGameSelected;
            //guiComponents.Add(changeTurnButton);

            ButtonComponent backButton = new ButtonComponent(ScreenManager.Game, 60, (int)(Settings.maximumResolution.Y - 200), new Rectangle(), GameResources.Inst().GetFont(EFont.MedievalBig), Settings.scaleW(79), Settings.scaleH(66), "HUD/hotseat_back");
            backButton.Actions += BackSelected;
            guiComponents.Add(backButton);

            int counter = 0;

            /*
             * Loading AI 
             */
            List<String> AIname = new List<String>();
            AIname.Add(Strings.Inst().GetString(TextEnum.MENU_HOT_SEAT_NO_AI));
            foreach (IComponentAI AI in CoreProviderAI.AI)
            {
                AIname.Add(AI.GetAIName());
            }
            Settings.PlayerState = AIname;

            /*
             * 
             */

            for (int loop1 = 0; loop1 < Settings.playerColors.Count; loop1++)
            {
                int playerNameID = GetRandomPlayerID();

                PlayerSettingRowComponent pSwitch = new PlayerSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y, GameResources.Inst().GetFont(EFont.PlayerNameFont), 200, 200, Settings.playerColors[loop1], Settings.playerColorNames[loop1], Strings.Inst().PlayerNames[playerNameID],
                                                                                  (counter == 0 || counter ==2) ? 1 : 0);
                playersSettings.Add(pSwitch);
                colorPosition.Y += playerSpace;

                if (counter <= 2)
                {
                    // first two users are active
                    pSwitch.setActive(true);
                }

                counter++;
            }

            //margin betweent sections
            colorPosition.Y += 80;

            mapType = new MapSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y, GameResources.Inst().GetFont(EFont.MedievalBig), 400, 200, Strings.Inst().GetString(TextEnum.MENU_HOT_SEAT_MAP_TYPE), new List<String>() { Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_TYPE_ISLAND), Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_TYPE_2_ISLANDS), Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_TYPE_SMALL_ISLANDS) });
            sourceKind = new MapSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y + 50, GameResources.Inst().GetFont(EFont.MedievalBig), 400, 200, Strings.Inst().GetString(TextEnum.MENU_HOT_SEAT_MAP_SOURCE), new List<String>() { Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_SOURCE_LOWLAND), Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_SOURCE_NORMAL), Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_SOURCE_WASTELAND) });
            secretProductivity = new MapSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y + 100, GameResources.Inst().GetFont(EFont.MedievalBig), 400, 200, Strings.Inst().GetString(TextEnum.MENU_HOT_SEAT_MAP_SECRET_KIND), new List<String>() { Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_KIND_HIDDEN), Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_KIND_VISIBLE), Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_KIND_HALF) });
            secretKind = new MapSettingRowComponent(ScreenManager.Game, (int)colorPosition.X, (int)colorPosition.Y + 150, GameResources.Inst().GetFont(EFont.MedievalBig), 400, 200, Strings.Inst().GetString(TextEnum.MENU_HOT_SEAT_MAP_SECRET_PRODUCTIVITY), new List<String>() { Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_PRODUCTIVITY_HIDDEN), Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_PRODUCTIVITY_VISIBLE), Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_PRODUCTIVITY_HALF)});

            guiComponents.Add(mapType);
            guiComponents.Add(sourceKind);
            guiComponents.Add(secretProductivity);
            guiComponents.Add(secretKind);

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

        private int GetRandomPlayerID()
        {
            int nameCount = Strings.Inst().PlayerNames.Length;
            int nameID = -1;
            bool uniqueID;
            do
            {
                nameID = random.Next() % nameCount;
                uniqueID = true;
                foreach (PlayerSettingRowComponent player in playersSettings)
                {
                    if (0 == player.GetName().CompareTo(Strings.Inst().PlayerNames[nameID]))
                    {
                        uniqueID = false;
                        continue;
                    }
                }
            } while (!uniqueID);

            return nameID;
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

            mapType.SetActiveRadio(true, 2);
            secretProductivity.SetActiveRadio(true, 0);
            secretProductivity.SetActiveRadio(true, 1);
            secretProductivity.SetActiveRadio(true, 2);
            

            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                p.Update(gameTime);
            }

            startGameButton.Update(gameTime);
        }


        #endregion

        int GetActivePlayerCount()
        {
            int playerCount = 0;

            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                if (p.isActive())
                {
                    playerCount++;
                }
            }
            return playerCount;
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void StartGameSelected(object sender, PlayerIndexEventArgs e)
        {
            MusicManager.Inst().PlaySound(SoundEnum.button1);
            //counting all players

            if (GetActivePlayerCount() < 1)
                return;

            saveScreenData();

            //ScreenManager.RemoveScreen(this);
            GameLoadScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(false));
            ScreenManager.RemoveScreen(this);
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            PlayerIndex index;
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Enter, null, out index))
                StartGameSelected(null, new PlayerIndexEventArgs(index));

            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Escape, null, out index))
            {
                BackSelected(null, new PlayerIndexEventArgs(index));
            }
        }

        /// <summary>
        /// Event handler for when the Back button is selected
        /// </summary>
        void BackSelected(object sender, PlayerIndexEventArgs e)
        {
            MusicManager.Inst().PlaySound(SoundEnum.button1);
            ScreenManager.AddScreen(new BackgroundScreen(), e.PlayerIndex);
            ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);

            ScreenManager.RemoveScreen(this);
        }


        private void saveScreenData()
        {
            int playerCount = 0;
            foreach (PlayerSettingRowComponent p in playersSettings)
            {
                Player pl = p.getPlayerSettings();
                
                if (pl != null)
                {
                    GameMaster.Inst().AddPlayer(pl);
                    playerCount++;
                }
            }

            string type = mapType.getSelectedSettings();
            String source = sourceKind.getSelectedSettings();
            String productivity = secretProductivity.getSelectedSettings();
            String kind = secretKind.getSelectedSettings();

            GameMaster.Inst().ResetGameSettings();
            GameMaster.Inst().SetMapSource(null);
            GameMaster.Inst().SetGameSettings(playerCount, type, source, productivity, kind);
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

            if( playerCount >= 1 )
                startGameButton.Draw(gameTime);

            spriteBatch.End();
        }

        #endregion
    }
}

