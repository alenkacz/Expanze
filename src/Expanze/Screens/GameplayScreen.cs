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
using Expanze.Utils;
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
        Map map;

        RenderTarget2D renderTarget;
        Texture2D shadowMap;

        //dummy texture for displaying players color in top hud
        Texture2D playerColorTexture;
        
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
            gMaster = GameMaster.Inst();

        }

        public void LoadAllThread()
        {
            playerColorTexture = GameResources.Inst().GetHudTexture(HUDTexture.PlayerColor);

            //render to texture
            PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, ScreenManager.GraphicsDevice.DisplayMode.Format, pp.DepthStencilFormat);

            GameState.game = ScreenManager.Game;

            map = new Map(ScreenManager.Game);
            gameComponents.Add(map);
            gameComponents.Add(PromptWindow.Inst());
            gameComponents.Add(Message.Inst());
            
            //gamelogic
            gMaster.StartGame(map);

            ButtonComponent changeTurnButton = new ButtonComponent(ScreenManager.Game, (int)(Settings.maximumResolution.X - 167), (int)(Settings.maximumResolution.Y - 161), new Rectangle(Settings.scaleW((int)(Settings.maximumResolution.X - 80)), Settings.scaleH((int)(Settings.maximumResolution.Y - 80)), Settings.scaleW(60), Settings.scaleH(60)), GameResources.Inst().GetFont(EFont.MedievalBig), Settings.scaleW(147), Settings.scaleH(141), "nextTurn");
            changeTurnButton.Actions += ChangeTurnButtonAction;
            guiComponents.Add(changeTurnButton);
            ButtonComponent menuHUDButton = new ButtonComponent(ScreenManager.Game, Settings.scaleW(20), Settings.scaleH(20), new Rectangle(Settings.scaleW(20), Settings.scaleH(20), Settings.scaleW(80), Settings.scaleH(80)), GameResources.Inst().GetFont(EFont.MedievalBig), Settings.scaleW(222), Settings.scaleH(225), "menu_button");
            menuHUDButton.Actions += MenuButtonAction;
            guiComponents.Add(menuHUDButton);
            MaterialsHUDComponent materialsHUDComp = new MaterialsHUDComponent(ScreenManager.Game, ScreenManager.Game.GraphicsDevice.Viewport.Width / 4, ScreenManager.Game.GraphicsDevice.Viewport.Height - 78, GameResources.Inst().GetFont(EFont.MedievalBig), 757, 148, "suroviny_hud");
            guiComponents.Add(materialsHUDComp);
            TopPlayerScoreComponent topPlayer = new TopPlayerScoreComponent();
            guiComponents.Add(topPlayer);
            MarketComponent marketHud = MarketComponent.Inst();
            ButtonComponent newMsg = new ButtonComponent(ScreenManager.Game, Settings.scaleW(30), (int)(Settings.maximumResolution.Y - 176), new Rectangle(Settings.scaleW(30), Settings.scaleH((int)(Settings.maximumResolution.Y - 176)), Settings.scaleW(70), Settings.scaleH(70)), GameResources.Inst().GetFont(EFont.MedievalBig), Settings.scaleW(151), Settings.scaleH(156), "newmessage");
            newMsg.Actions += MarketButtonAction;
            guiComponents.Add(newMsg);

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

            MarketComponent.Inst().Initialize();
            MarketComponent.Inst().LoadContent();

            InputManager im = InputManager.Inst();
            String stateGame = "game";
            if (im.AddState(stateGame))
            {
                GameAction pause = new GameAction("pause", GameAction.ActionKind.OnlyInitialPress);
                GameAction nextTurn = new GameAction("nextturn", GameAction.ActionKind.OnlyInitialPress);
                GameAction market = new GameAction("market", GameAction.ActionKind.OnlyInitialPress);
                GameAction cameraleft = new GameAction("cameraleft", GameAction.ActionKind.Normal);
                GameAction cameraright = new GameAction("cameraright", GameAction.ActionKind.Normal);
                GameAction cameraup = new GameAction("cameraup", GameAction.ActionKind.Normal);
                GameAction cameradown = new GameAction("cameradown", GameAction.ActionKind.Normal);
                GameAction cameratop = new GameAction("cameratop", GameAction.ActionKind.Normal);
                GameAction camerabottom = new GameAction("camerabottom", GameAction.ActionKind.Normal);
     
                im.MapToKey(stateGame, pause, Keys.Escape);
                im.MapToKey(stateGame, nextTurn, Keys.Tab);
                im.MapToKey(stateGame, market, Keys.M);
                im.MapToKey(stateGame, cameraleft, Keys.Left);
                im.MapToKey(stateGame, cameraright, Keys.Right);
                im.MapToKey(stateGame, cameraup, Keys.Up);
                im.MapToKey(stateGame, cameradown, Keys.Down);
                im.MapToKey(stateGame, cameratop, Keys.PageUp);
                im.MapToKey(stateGame, camerabottom, Keys.PageDown);
            }
            im.SetActiveState(stateGame);

            String stateGameWindow = "gamewindow";
            if (im.AddState(stateGameWindow))
            {
                GameAction confirm = new GameAction("confirm", GameAction.ActionKind.OnlyInitialPress);
                GameAction close = new GameAction("close", GameAction.ActionKind.OnlyInitialPress);
                GameAction left = new GameAction("left", GameAction.ActionKind.OnlyInitialPress);
                GameAction right = new GameAction("right", GameAction.ActionKind.OnlyInitialPress);
                GameAction quickChangeSources = new GameAction("changesources", GameAction.ActionKind.OnlyInitialPress);
                GameAction canChangeSources = new GameAction("canchange", GameAction.ActionKind.OnlyInitialPress);

                im.MapToKey(stateGameWindow, confirm, Keys.Enter);
                im.MapToKey(stateGameWindow, close, Keys.Escape);
                im.MapToKey(stateGameWindow, left, Keys.Left);
                im.MapToKey(stateGameWindow, right, Keys.Right);
                im.MapToKey(stateGameWindow, quickChangeSources, Keys.T);
                im.MapToKey(stateGameWindow, canChangeSources, Keys.R);
            }

            String stateMessage = "gamemessage";
            if (im.AddState(stateMessage))
            {
                GameAction close = new GameAction("close", GameAction.ActionKind.OnlyInitialPress);
                GameAction cheatsources = new GameAction("cheatsources", GameAction.ActionKind.OnlyInitialPress);
                GameAction cheatpoints = new GameAction("cheatpoints", GameAction.ActionKind.OnlyInitialPress);
                im.MapToKey(stateMessage, close, Keys.Escape);
                im.MapToKey(stateMessage, close, Keys.Enter);
                im.MapToKey(stateMessage, cheatsources, Keys.F11);
                im.MapToKey(stateMessage, cheatpoints, Keys.F12);
            }

            String stateMarket = "gamemarket";
            if (im.AddState(stateMarket))
            {
                GameAction ok = new GameAction("ok", GameAction.ActionKind.OnlyInitialPress);
                GameAction close = new GameAction("close", GameAction.ActionKind.OnlyInitialPress);

                GameAction right = new GameAction("right", GameAction.ActionKind.OnlyInitialPress);
                GameAction left = new GameAction("left", GameAction.ActionKind.OnlyInitialPress);
                im.MapToKey(stateMarket, close, Keys.Escape);
                im.MapToKey(stateMarket, ok, Keys.Enter);
                im.MapToKey(stateMarket, close, Keys.M);

                im.MapToKey(stateMarket, right, Keys.Right);
                im.MapToKey(stateMarket, left, Keys.Left);
            }

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
            foreach (GameComponent gameComponent in gameComponents)
            {
                gameComponent.UnloadContent();
            }

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.UnloadContent();
            }

            MarketComponent.Inst().UnloadContent();
        }


        #endregion

        #region HUDEventHandlers

        /// <summary>
        /// Event handler for when the menu button is selected
        /// </summary>
        void MenuButtonAction(object sender, PlayerIndexEventArgs e)
        {
            GameMaster.Inst().SetPausedNew(true);
        }

        /// <summary>
        /// Event handler for when the menu button is selected
        /// </summary>
        void ChangeTurnButtonAction(object sender, PlayerIndexEventArgs e)
        {
            NextTurn();
        }

        void NextTurn()
        {
            if (GameMaster.Inst().CanNextTurn())
            {
                GameMaster.Inst().NextTurn();
            }
        }

        /// <summary>
        /// Event handler for when the market button is selected
        /// </summary>
        void MarketButtonAction(object sender, PlayerIndexEventArgs e)
        {
            MarketWindowOpenClose();
        }

        void MarketWindowOpenClose()
        {
            // market can not be opened during first phase of the game - building first towns
            if (GameMaster.Inst().GetState() == EGameState.StateGame &&
                !GameMaster.Inst().GetActivePlayer().GetIsAI())
            {
                MarketComponent.Inst().SetIsActive(!MarketComponent.Inst().getIsActive());
            }
            else
            {
                Message.Inst().Show(Strings.GAME_ALERT_TITLE_MARKET_BAD_TURN, Strings.GAME_ALERT_DESCRIPTION_MARKET_BAD_TURN, GameResources.Inst().GetHudTexture(HUDTexture.IconMarket));
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
                InputManager.Inst().Update();

                foreach (GuiComponent guiComponent in guiComponents)
                {
                    guiComponent.Update(gameTime);
                }

                MarketComponent.Inst().Update(gameTime);
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

            GameState.CurrentKeyboardState = keyboardState;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (InputManager.Inst().GetGameAction("game", "pause").IsPressed() || GameMaster.Inst().IsPausedNew())
            {
                GameMaster.Inst().SetPaused(true);
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }

            if (InputManager.Inst().GetGameAction("game", "nextturn").IsPressed())
                NextTurn();

            if (InputManager.Inst().GetGameAction("game", "market").IsPressed())
                MarketWindowOpenClose();

            if (InputManager.Inst().GetGameAction("gamemessage", "cheatsources").IsPressed())
                GameMaster.Inst().GetActivePlayer().PayForSomething(new SourceAll(-1000));
            if (InputManager.Inst().GetGameAction("gamemessage", "cheatpoints").IsPressed())
                GameMaster.Inst().GetActivePlayer().AddPoints(10);

            if (GameMaster.Inst().IsWinnerNew())
            {
                VictoryScreen.Load(ScreenManager, true, ControllingPlayer,
                               new GameScreen[] {new MainMenuScreen() });
            }
        }

        int frames = 0;
        int showFrames = 60;
        int timeMS = 0;

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

            if (MarketComponent.Inst().getIsActive())
            {
                MarketComponent.Inst().Draw(gameTime, true);
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
            //ScreenManager.GraphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
           
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

            if (MarketComponent.Inst().getIsActive())
            {
                MarketComponent.Inst().Draw(gameTime, false);
            }

            frames++;
            timeMS += gameTime.ElapsedGameTime.Milliseconds;
            if (timeMS > 3000)
            {
                showFrames = frames / 3;
                timeMS -= 3000;
                frames = 0;
            }

            /*
            if (gameTime.ElapsedGameTime.Milliseconds != 0)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.GameFont), showFrames + " ", new Vector2(12, 12), Color.Black);
                spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.GameFont), showFrames + " ", new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.GameFont), InputManager.Inst().GetActiveState(), new Vector2(10, 60), Color.White);
                spriteBatch.End();
            }*/

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
