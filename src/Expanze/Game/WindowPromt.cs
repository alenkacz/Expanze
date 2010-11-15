using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Expanze.Gameplay.Map;
using CorePlugin;

namespace Expanze
{
    enum WindowKind { Promt, Alert };
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

        private WindowKind kind;
        public delegate void ActionDelegate();
        ActionDelegate action;
        private String text;
        private int argInt1;
        private int argInt2;
        private SourceAll winCost;

        public WindowPromt()
        {
            spriteBatch = GameState.spriteBatch;
            bgPos = new Vector2(0, 0);
            active = false;

            noPick = new PickVariables(Color.YellowGreen);
            yesPick = new PickVariables(Color.Tomato);
        }

        public void showPromt(String text, ActionDelegate action, SourceAll winCost)
        {
            this.winCost = winCost; // if is not used, its SourceAll(0);
            this.text = text;
            this.action = action;
            kind = WindowKind.Promt;
            active = true;
        }

        public void setArgInt1(int arg) { argInt1 = arg; }
        public void setArgInt2(int arg) { argInt2 = arg; }
        public void showAlert(String text)
        {
            this.text = text;
            kind = WindowKind.Alert;
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
            {
                yesPick.pickNewPress = false;
                active = false;
                if (kind == WindowKind.Promt)
                {
                    action();
                }
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

                if (kind == WindowKind.Promt)
                {
                    if (winCost != new SourceAll(0))
                    {
                        spriteBatch.DrawString(GameState.materialsNewFont, winCost.corn.ToString(), new Vector2(bgPos.X + 30, bgPos.Y + 200), Color.White);
                        spriteBatch.DrawString(GameState.materialsNewFont, winCost.meat.ToString(), new Vector2(bgPos.X + 90, bgPos.Y + 200), Color.White);
                        spriteBatch.DrawString(GameState.materialsNewFont, winCost.stone.ToString(), new Vector2(bgPos.X + 150, bgPos.Y + 200), Color.White);
                        spriteBatch.DrawString(GameState.materialsNewFont, winCost.wood.ToString(), new Vector2(bgPos.X + 210, bgPos.Y + 200), Color.White);
                        spriteBatch.DrawString(GameState.materialsNewFont, winCost.ore.ToString(), new Vector2(bgPos.X + 270, bgPos.Y + 200), Color.White);
                    }
                }

                spriteBatch.End();
            }
        }

        public void BuildTown()
        {
            switch(GameState.map.BuildTown(argInt1))
            {
                case TownBuildError.AlreadyBuild :
                    showAlert(Strings.ALERT_TITLE_TOWN_IS_BUILD);
                    break;
                case TownBuildError.NoSources :
                    showAlert(Strings.ALERT_TITLE_NOT_ENOUGH_SOURCES);
                    break;
                case TownBuildError.NoPlayerRoad:
                    showAlert(Strings.ALERT_TITLE_NO_ROAD_IS_CLOSE);
                    break;
                case TownBuildError.OtherTownIsClose:
                    showAlert(Strings.ALERT_TITLE_OTHER_TOWN_IS_CLOSE);
                    break;
            }
        }

        public void BuildRoad()
        {
            switch (GameState.map.BuildRoad(argInt1))
            {
                case RoadBuildError.NoSources:
                    showAlert(Strings.ALERT_TITLE_NOT_ENOUGH_SOURCES);
                    break;
                case RoadBuildError.AlreadyBuild:
                    showAlert(Strings.ALERT_TITLE_ROAD_IS_BUILD);
                    break;
                case RoadBuildError.NoPlayerRoadOrTown:
                    showAlert(Strings.ALERT_TITLE_NO_ROAD_OR_TOWN_IS_CLOSE);
                    break;
            }
        }
    }
}
