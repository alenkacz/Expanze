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

        ContentManager content;
        
        List<GameComponent> gameComponents = new List<GameComponent>();
        List<GuiComponent> guiComponents = new List<GuiComponent>();
        CustomCursor cursorComp;


        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        float pauseAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            gMaster = GameMaster.getInstance();
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            GameState.gameFont = content.Load<SpriteFont>("gamefont");
            GameState.hudMaterialsFont = content.Load<SpriteFont>("hudMaterialsFont");

            //gamelogic
            gMaster.startGame();

            //render to texture
            PresentationParameters pp = ScreenManager.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, ScreenManager.GraphicsDevice.DisplayMode.Format, pp.DepthStencilFormat);
            //renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, 1024, 1024, false, ScreenManager.GraphicsDevice.DisplayMode.Format, pp.DepthStencilFormat);
    
            Map mapComp;
            GameState.game = ScreenManager.Game;
            mapComp = new Map(ScreenManager.Game);
            gameComponents.Add(mapComp);

            ButtonComponent changeTurnButton = new ButtonComponent(ScreenManager.Game, ScreenManager.Game.GraphicsDevice.Viewport.Width - 180, ScreenManager.Game.GraphicsDevice.Viewport.Height - 50, GameState.gameFont, 150, 40, "button");
            guiComponents.Add(changeTurnButton);
            MenuButtonComponent menuHUDButton = new MenuButtonComponent(ScreenManager.Game, 10, 10, GameState.gameFont, 232, 225, "menu_button");
            guiComponents.Add(menuHUDButton);
            MaterialsHUDComponent materialsHUDComp = new MaterialsHUDComponent(ScreenManager.Game, ScreenManager.Game.GraphicsDevice.Viewport.Width/4, ScreenManager.Game.GraphicsDevice.Viewport.Height - 78, GameState.gameFont, 232, 225, "suroviny_hud");
            guiComponents.Add(materialsHUDComp);
            GuiComponent usersHud = new GuiComponent(ScreenManager.Game, ScreenManager.Game.GraphicsDevice.Viewport.Width -100, 0, GameState.gameFont, 232, 225, "suroviny_hud");
            guiComponents.Add(usersHud);
            //gameComponents.Add(buttonComp);

            cursorComp = new CustomCursor(ScreenManager.Game);

            foreach(GameComponent gameComponent in gameComponents)
            {
                gameComponent.Initialize();
                gameComponent.LoadContent();
            }

            foreach (GuiComponent guiComponent in guiComponents)
            {
                guiComponent.Initialize();
                guiComponent.LoadContent();
            }

            //must be last - to be on the highest layer
            cursorComp.Initialize();
            cursorComp.LoadContent();

            //simulating loading screens
            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
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

            cursorComp.UnloadContent();
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
                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;

                enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - GameState.gameFont.MeasureString("Insert Gameplay Here").X / 2, 
                    200);

                enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);

                foreach (GameComponent gameComponent in gameComponents)
                {
                    gameComponent.Update(gameTime);
                }

                foreach (GuiComponent guiComponent in guiComponents)
                {
                    guiComponent.Update(gameTime);
                }

                cursorComp.Update(gameTime);
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

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected || GameMaster.getInstance().isPaused())
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;
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
                                               Color.Black, 0, 0);

           
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
                guiComponent.Draw(gameTime);
            }

            cursorComp.Draw(gameTime);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.DrawString(GameState.hudMaterialsFont, gMaster.getActivePlayer().getName(), Settings.playerNamePosition, gMaster.getActivePlayer().getColor());
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
