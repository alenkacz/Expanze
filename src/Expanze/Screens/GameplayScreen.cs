#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Expanze.Gameplay.Map;
using CorePlugin;
#endregion

namespace Expanze
{
    /// <summary>
    ///  Main gameplay screen
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        GameMaster gMaster;

        RenderTarget2D renderTarget;
        Texture2D shadowMap;

        //dummy texture for displaying players color in top hud
        Texture2D playerColorTexture;

        ContentManager content;
        
        List<GameComponent> gameComponents = new List<GameComponent>();
        List<GuiComponent> guiComponents = new List<GuiComponent>();

        bool isAI;
        bool isGameLoaded;


        Random random = new Random();

        float pauseAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(bool AI)
        {
            isAI = AI;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            gMaster = GameMaster.getInstance();

        }

        public void LoadAllThread()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            GameState.gameFont = content.Load<SpriteFont>("gamefont");
            GameState.playerNameFont = content.Load<SpriteFont>("playername");
            GameState.hudMaterialsFont = content.Load<SpriteFont>("hudMaterialsFont");
            GameState.materialsNewFont = content.Load<SpriteFont>("materialsNewFont");
            GameState.medievalSmall = content.Load<SpriteFont>("Fonts/medievalSmall");
            GameState.medievalMedium = content.Load<SpriteFont>("Fonts/medievalMedium");
            GameState.medievalBig = content.Load<SpriteFont>("Fonts/medievalBig");

            //playerColorTexture = new Texture2D(ScreenManager.GraphicsDevice, (int)Settings.playerColorSize.X, (int)Settings.playerColorSize.Y, false, SurfaceFormat.Color);
            playerColorTexture = ScreenManager.Game.Content.Load<Texture2D>("pcolor");

            //render to texture
            PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, ScreenManager.GraphicsDevice.DisplayMode.Format, pp.DepthStencilFormat);
            //renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, 1024, 1024, false, ScreenManager.GraphicsDevice.DisplayMode.Format, pp.DepthStencilFormat);

            Map mapComp;
            GameState.game = ScreenManager.Game;
            GameResources.Inst().LoadContent();

            mapComp = new Map(ScreenManager.Game);
            gameComponents.Add(mapComp);
            GameState.windowPromt = new WindowPromt();
            gameComponents.Add(GameState.windowPromt);
            gameComponents.Add(PromptWindow.Inst());
            //gamelogic
            gMaster.startGame(isAI, mapComp);

            ButtonComponent changeTurnButton = new ButtonComponent(ScreenManager.Game, (int)(Settings.maximumResolution.X - 167), (int)(Settings.maximumResolution.Y - 161), new Rectangle(Settings.scaleW((int)(Settings.maximumResolution.X - 80)), Settings.scaleH((int)(Settings.maximumResolution.Y - 80)), Settings.scaleW(60), Settings.scaleH(60)), GameState.gameFont, Settings.scaleW(147), Settings.scaleH(141), "nextTurn");
            changeTurnButton.Actions += ChangeTurnButtonAction;
            guiComponents.Add(changeTurnButton);
            ButtonComponent menuHUDButton = new ButtonComponent(ScreenManager.Game, Settings.scaleW(20), Settings.scaleH(20), new Rectangle(Settings.scaleW(20), Settings.scaleH(20), Settings.scaleW(80), Settings.scaleH(80)), GameState.gameFont, Settings.scaleW(222), Settings.scaleH(225), "menu_button");
            menuHUDButton.Actions += MenuButtonAction;
            guiComponents.Add(menuHUDButton);
            MaterialsHUDComponent materialsHUDComp = new MaterialsHUDComponent(ScreenManager.Game, ScreenManager.Game.GraphicsDevice.Viewport.Width / 4, ScreenManager.Game.GraphicsDevice.Viewport.Height - 78, GameState.gameFont, 757, 148, "suroviny_hud");
            guiComponents.Add(materialsHUDComp);
            GuiComponent usersHud = new GuiComponent(ScreenManager.Game, (int)(Settings.maximumResolution.X - 670), 10, GameState.gameFont, Settings.scaleW(660), Settings.scaleH(46), "hud-top");
            guiComponents.Add(usersHud);
            MarketComponent marketHud = MarketComponent.getInstance();
            //guiComponents.Add(marketHud);
            //GuiComponent marketHud = new GuiComponent(ScreenManager.Game, 100, 10, GameState.gameFont, Settings.scaleW(500), Settings.scaleH(500), "market_bg");
            //guiComponents.Add(marketHud);
            ButtonComponent newMsg = new ButtonComponent(ScreenManager.Game, Settings.scaleW(30), (int)(Settings.maximumResolution.Y - 176), new Rectangle(Settings.scaleW(30), Settings.scaleH((int)(Settings.maximumResolution.Y - 176)), Settings.scaleW(70), Settings.scaleH(70)), GameState.gameFont, Settings.scaleW(151), Settings.scaleH(156), "newmessage");
            newMsg.Actions += MarketButtonAction;
            guiComponents.Add(newMsg);
            //gameComponents.Add(buttonComp);

            foreach (GameComponent gameComponent in gameComponents)
            {
                gameComponent.Initialize();
                gameComponent.LoadContent();
            }

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Initialize();
                guiComponent.LoadContent();
            }

            MarketComponent.getInstance().Initialize();
            MarketComponent.getInstance().LoadContent();

            //simulating loading screens
            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            isGameLoaded = true;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            isGameLoaded = false;
            ThreadStart loadingThreadStart = new ThreadStart(this.LoadAllThread);
            Thread loadingThread = new Thread(loadingThreadStart);
            loadingThread.Start();

            while (!isGameLoaded)
            {
                Thread.Sleep(100);
            }
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
            foreach (GameComponent gameComponent in gameComponents)
            {
                gameComponent.UnloadContent();
            }

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.UnloadContent();
            }

            MarketComponent.getInstance().UnloadContent();
        }


        #endregion

        #region HUDEventHandlers

        /// <summary>
        /// Event handler for when the menu button is selected
        /// </summary>
        void MenuButtonAction(object sender, PlayerIndexEventArgs e)
        {
            GameMaster.getInstance().setPausedNew(true);
        }

        /// <summary>
        /// Event handler for when the menu button is selected
        /// </summary>
        void ChangeTurnButtonAction(object sender, PlayerIndexEventArgs e)
        {
            if (GameMaster.getInstance().getState() == EGameState.StateGame &&
                !GameMaster.getInstance().getActivePlayer().getIsAI())
            {
                GameMaster.getInstance().NextTurn();
            }
            else
            {
                // ble ble GameState.windowPromt.showAlert("Musíš nejdøíve postavit mìsto.");
            }
        }

        /// <summary>
        /// Event handler for when the market button is selected
        /// </summary>
        void MarketButtonAction(object sender, PlayerIndexEventArgs e)
        {
            // market can not be opened during first phase of the game - building first towns
            if (GameMaster.getInstance().getState() == EGameState.StateGame)
            {

                if (MarketComponent.isActive)
                {
                    MarketComponent.isActive = false;
                    //guiComponents.Remove(MarketComponent.getInstance());
                }
                else
                {
                    MarketComponent.isActive = true;
                }
            }
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {

            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                foreach (GameComponent gameComponent in gameComponents)
                {
                    gameComponent.Update(gameTime);
                }

                gMaster.Update(gameTime);

                foreach (GuiComponent guiComponent in guiComponents)
                {
                    guiComponent.Update(gameTime);
                }

                MarketComponent.getInstance().Update(gameTime);
            }

            
        }

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
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected || GameMaster.getInstance().isPausedNew())
            {
                GameMaster.getInstance().setPaused(true);
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }

            if (GameMaster.getInstance().isWinnerNew())
            {
                VictoryScreen.Load(ScreenManager, true, ControllingPlayer,
                               new GameScreen[] { new BackgroundScreen(), new MainMenuScreen() });
            }
        }



        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            //render to texture
            ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget);
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);
            foreach (GameComponent gameComponent in gameComponents)
            {
                gameComponent.DrawPickableAreas();
            }

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Draw(gameTime, true);
            }

            if (MarketComponent.isActive)
            {
                MarketComponent.getInstance().Draw(gameTime, true);
            }

            //render to texture
            ScreenManager.GraphicsDevice.SetRenderTarget(null);
            shadowMap = renderTarget;
            Color[] c = new Color[shadowMap.Height * shadowMap.Width];
            shadowMap.GetData<Color>(c);

            Color color = Color.Black;
            int x = GameState.CurrentMouseState.X, y = GameState.CurrentMouseState.Y;
            if(x > 0 && y > 0 && x < shadowMap.Width && y < shadowMap.Height)
                color = c[x + y * shadowMap.Width];
            foreach (GameComponent gameComponent in gameComponents)
            {
                gameComponent.HandlePickableAreas(color);
            }

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.SkyBlue, /*new Color(33, 156, 185), */0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            /*
            spriteBatch.Begin();
            spriteBatch.Draw(GameResources.Inst().getHudTexture(HUDTexture.BackgroundWater), new Vector2(0, 0), Color.Gray);
            spriteBatch.End();*/
           
            foreach (GameComponent gameComponent in gameComponents)
            {
                gameComponent.Draw(gameTime);
            }

            foreach (GameComponent gameComponent in gameComponents)
            {
                gameComponent.Draw2D();
            }

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Draw(gameTime, false);
            }

            if (MarketComponent.isActive)
            {
                MarketComponent.getInstance().Draw(gameTime, false);
            }

            

            //player name and color rectangle
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            spriteBatch.DrawString(GameState.playerNameFont, gMaster.getActivePlayer().getPoints().ToString(), new Vector2(Settings.playerNamePosition.X - 300, Settings.playerNamePosition.Y), Color.White);
            spriteBatch.DrawString(GameState.playerNameFont, gMaster.getActivePlayer().getName(), Settings.playerNamePosition, Color.White);
            spriteBatch.Draw(playerColorTexture, Settings.playerColorPosition, gMaster.getActivePlayer().getColor());
            
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            
        }


        #endregion
    }
}
