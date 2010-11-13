using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Expanze.Gameplay.Map;

namespace Expanze
{
    class WindowPromt : GameComponent
    {
        private SpriteBatch spriteBatch;
        private bool drawingPickableAreas = false;
        private bool active = false;
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

        private String text;
        
        public WindowPromt()
        {
            spriteBatch = GameState.spriteBatch;
            bgPos = new Vector2(0, 0);
            active = false;

            noPick = new PickVariables(Color.YellowGreen);
            yesPick = new PickVariables(Color.Tomato);
        }

        public void showAlert(String text)
        {
            this.text = text;
            active = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (content == null)
                content = new ContentManager(GameState.game.Services, "Content");

            background = content.Load<Texture2D>("HUD/WindowPromt");
            no = content.Load<Texture2D>("HUD/NOPromt");
            yes = content.Load<Texture2D>("HUD/OKPromt");
            pickTexture = content.Load<Texture2D>("HUD/PickPromt");
            bgPos = new Vector2((Settings.activeResolution.X - background.Width) / 2,
                                   (Settings.activeResolution.Y - background.Height) / 2);
            int border = 50;
            yesPos = new Vector2(bgPos.X + border, bgPos.Y + background.Height - border - yes.Height);
            noPos = new Vector2(bgPos.X - no.Width - border + background.Width, bgPos.Y + background.Height - border - no.Height);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            content.Dispose();
        }

        public override void HandlePickableAreas(Color c)
        {
            Map.SetPickVariables(c == noPick.pickColor, noPick);
            Map.SetPickVariables(c == yesPick.pickColor, yesPick);

            if (yesPick.pickNewPress)
                active = false;
        }

        override public void DrawPickableAreas()
        {
            drawingPickableAreas = true;
            Draw2D();
            drawingPickableAreas = false;
        }

        override public void Draw2D()
        {
            if (active)
            {
                Color color;
                if (drawingPickableAreas)
                    color = Color.Black;
                else
                    color = Color.White;

                spriteBatch.Begin();
                spriteBatch.Draw(background, bgPos, color);

                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTexture, yesPos, yesPick.pickColor);
                else
                    spriteBatch.Draw(yes, yesPos, Color.White);

                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTexture, noPos, noPick.pickColor);
                else
                    spriteBatch.Draw(no, noPos, Color.White);
                spriteBatch.DrawString(GameState.materialsNewFont, text, new Vector2(bgPos.X + 20, bgPos.Y + 100), Color.LightBlue);
                spriteBatch.End();
            }
        }


    }
}
