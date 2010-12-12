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
        private Texture2D yes;
        private Texture2D pickTexture;
        private Vector2 bgPos;
        private Vector2 yesPos;
        private Vector2 noPos;
        private PickVariables noPick;
        private PickVariables yesPick;
        private ContentManager content;

        private Texture2D icon;
        private String title;
        private String description;

        private bool active = false;
        private int timeActive;
        private const int ACTIVE_LIMIT = 2500;

        public Message()
        {
            spriteBatch = GameState.spriteBatch;
            bgPos = new Vector2(0, 0);
            active = false;
            timeActive = ACTIVE_LIMIT;

            noPick = new PickVariables(Color.YellowGreen);
            yesPick = new PickVariables(Color.Tomato);
        }

        public bool getIsActive() {return active;}


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (active)
            {
                if (timeActive > 0)
                    timeActive -= gameTime.ElapsedGameTime.Milliseconds;
                else
                {
                    active = false;
                    timeActive = ACTIVE_LIMIT;
                }
            }
        }

        public void showAlert(String title, String description, Texture2D icon)
        {
            this.icon = icon;
            this.title = title;
            this.description = description;
            active = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (content == null)
                content = new ContentManager(GameState.game.Services, "Content");

            background = content.Load<Texture2D>("HUD/messageBG");
            no = content.Load<Texture2D>("HUD/NOPromt");
            yes = content.Load<Texture2D>("HUD/OKPromt");
            pickTexture = content.Load<Texture2D>("HUD/PickPromt");
            bgPos = new Vector2((Settings.maximumResolution.X - background.Width) / 2,
                                   (Settings.maximumResolution.Y - background.Height) / 2 - 50);
            int border = 12;
            yesPos = new Vector2(bgPos.X + (background.Width - yes.Width) / 2, bgPos.Y + background.Height - border - yes.Height);
            noPos = new Vector2(bgPos.X + (background.Width - yes.Width) / 2, bgPos.Y + background.Height - border - no.Height);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            //content.Dispose();
        }

        public override void HandlePickableAreas(Color c)
        {
            Map.SetPickVariables(c == noPick.pickColor, noPick);
            Map.SetPickVariables(c == yesPick.pickColor, yesPick);

            if (yesPick.pickNewPress || GameState.CurrentKeyboardState.IsKeyDown(Keys.Enter))
            {
                yesPick.pickNewPress = false;
                active = false;
            } else if(noPick.pickNewPress)
            {
                noPick.pickNewPress = false;
                active = false;
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
            if (active)
            {
                Color color;
                if (drawingPickableAreas)
                    color = Color.Black;
                else
                    color = Color.White;

                spriteBatch.Begin(SpriteSortMode.Deferred, (drawingPickableAreas) ? BlendState.Opaque : BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            
                spriteBatch.Draw(background, bgPos, color);

                /*
                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTexture, yesPos, yesPick.pickColor);
                else
                    spriteBatch.Draw(yes, yesPos, Color.White);*/

                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTexture, noPos, noPick.pickColor);
                else
                    spriteBatch.Draw(no, noPos, Color.White);

                TextWrapping.DrawStringCentered(title, GameState.medievalBig, Color.LightBlue, bgPos.X, bgPos.Y + 10, background.Width);
                TextWrapping.DrawStringIntoRectangle(description,
                    GameState.medievalSmall, Color.LightSteelBlue, bgPos.X + 20, bgPos.Y + 55, background.Width - 140);

                spriteBatch.Draw(icon, new Vector2(bgPos.X + background.Width - icon.Width - 20, bgPos.Y + 30), color);

                spriteBatch.End();
            }
        }
    }
}
