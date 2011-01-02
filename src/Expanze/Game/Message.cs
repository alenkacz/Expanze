using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Expanze.Gameplay.Map;
using CorePlugin;
using Expanze.Utils;
using Microsoft.Xna.Framework.Input;

namespace Expanze
{
    enum MessageKind { Promt, Alert };
    class Message : GameComponent
    {
        private SpriteBatch spriteBatch;
        private bool drawingPickableAreas = false;
        
        private Texture2D background;
        private Texture2D no;
        private Texture2D pickTexture;
        private Vector2 bgPos;
        private Vector2 noPos;
        private PickVariables noPick;

        private MessageItem messageActive;  /// message on the screen
        private Queue<MessageItem> queue;

        private bool disabled; /// Can be popup messages?
        private int timeActive;
        private const int ACTIVE_LIMIT = 3200;

        private static Message instance = null;

        public static Message Inst()
        {
            if (instance == null)
            {
                instance = new Message();
            }

            return instance;
        }

        private Message()
        {
            spriteBatch = GameState.spriteBatch;
            bgPos = new Vector2(0, 0);

            timeActive = ACTIVE_LIMIT;
            disabled = false;

            noPick = new PickVariables(Color.YellowGreen);

            queue = new Queue<MessageItem>();
        }

        public bool GetIsActive() {return messageActive != null;}


        private void NextMessage()
        {
            if (queue.Count == 0)
            {
                messageActive = null;
            }
            else
                messageActive = queue.Dequeue();

            timeActive = ACTIVE_LIMIT;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (messageActive != null)
            {
                if (timeActive > 0)
                    timeActive -= gameTime.ElapsedGameTime.Milliseconds;
                else
                {
                    NextMessage();
                }
            }

            if (GameState.CurrentKeyboardState.IsKeyDown(Keys.P) && !disabled)
            {
                disabled = true;
            }
        }

        public void Show(String title, String description, Texture2D icon)
        {
            if (disabled)
                return;

            MessageItem item = new MessageItem(title, description, icon);
            if (messageActive == null)
                messageActive = item;
            else
                queue.Enqueue(item);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            background = GameResources.Inst().GetHudTexture(HUDTexture.BackgroundMessageWindow);
            no = GameResources.Inst().GetHudTexture(HUDTexture.ButtonNo);
            pickTexture = GameResources.Inst().GetHudTexture(HUDTexture.PickWindowPrompt);
            bgPos = new Vector2((Settings.maximumResolution.X - background.Width) / 2,
                                   (Settings.maximumResolution.Y - background.Height) / 2 - 50);
            int border = 12;
            noPos = new Vector2(bgPos.X + (background.Width - no.Width) / 2, bgPos.Y + background.Height - border - no.Height);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void HandlePickableAreas(Color c)
        {
            Map.SetPickVariables(c == noPick.pickColor, noPick);

            if ((noPick.pickNewPress || GameState.CurrentKeyboardState.IsKeyDown(Keys.Enter)) &&
                 ACTIVE_LIMIT - timeActive > 200)
            {
                noPick.pickNewPress = false;
                NextMessage();
            }
        }

        override public void DrawPickableAreas()
        {
            drawingPickableAreas = true;
            Draw2D();
            drawingPickableAreas = false;
        }

        override public void Draw2D()
        {
            //drawingPickableAreas = true;
            if (messageActive != null)
            {
                Color color;
                if (drawingPickableAreas)
                    color = Color.Black;
                else
                    color = Color.White;

                spriteBatch.Begin(SpriteSortMode.Deferred, (drawingPickableAreas) ? BlendState.Opaque : BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            
                spriteBatch.Draw(background, bgPos, color);

                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTexture, noPos, noPick.pickColor);
                else
                    spriteBatch.Draw(no, noPos, Color.White);

                TextWrapping.DrawStringCentered(messageActive.getTitle(), GameState.medievalBig, Color.LightBlue, bgPos.X, bgPos.Y + 10, background.Width);
                TextWrapping.DrawStringIntoRectangle(messageActive.getDescription(),
                    GameState.medievalSmall, Color.LightSteelBlue, bgPos.X + 20, bgPos.Y + 55, background.Width - 140);

                if(messageActive.getIcon() != null)
                    spriteBatch.Draw(messageActive.getIcon(), new Vector2(bgPos.X + background.Width - messageActive.getIcon().Width - 20, bgPos.Y + 30), color);

                spriteBatch.End();
            }
        }
    }
}
