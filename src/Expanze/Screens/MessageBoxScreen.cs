﻿#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Expanze
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : GameScreen
    {
        #region Fields

        string message;
        Texture2D gradientTexture;
        Vector2 yesPosition;
        Vector2 noPosition;
        bool yesActive;
        bool noActive;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message)
            : this(message, true)
        {
            yesActive = false;
            noActive = false;
        }


        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText)
        {

            this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gradientTexture = content.Load<Texture2D>("gradient");
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input.IsMenuMouseHover(new Rectangle((int)yesPosition.X, (int)yesPosition.Y, 200, 100)))
                yesActive = true;
            else
                yesActive = false;

            if (input.IsMenuMouseHover(new Rectangle((int)noPosition.X, (int)noPosition.Y, 200, 100)))
                noActive = true;
            else
                noActive = false;


            if(input.IsMenuMouseClicked(new Rectangle((int)noPosition.X,(int)noPosition.Y, 200,100)))
            {
                if( Cancelled != null )
                    Cancelled(this, new PlayerIndexEventArgs(PlayerIndex.One));

                ExitScreen();
            }

            if (input.IsMenuMouseClicked(new Rectangle((int)yesPosition.X, (int)yesPosition.Y, 200, 100)))
            {
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(PlayerIndex.One));

                ExitScreen();
            }

            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 9 / 10);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (Settings.maximumResolution - textSize) / 2;
            textPosition.Y -= 40;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;
            
            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          1000,
                                                          180);

            yesPosition = new Vector2(570, textPosition.Y + 80);
            noPosition = new Vector2(870, textPosition.Y + 80);

            // Fade the popup alpha during transitions.
            Color color = Color.BurlyWood * TransitionAlpha;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);

            // Draw the background rectangle.
            //spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            // Draw the message box text.
            spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.DrawString(font, Strings.MENU_COMMON_YES, yesPosition, (yesActive) ? Color.Yellow : color);
            spriteBatch.DrawString(font, Strings.MENU_COMMON_NO, noPosition, (noActive) ? Color.Yellow : color);

            spriteBatch.End();
        }


        #endregion
    }
}
