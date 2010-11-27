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
    class PromptWindow : GameComponent
    {
        private SpriteBatch spriteBatch;
        private bool drawingPickableAreas = false;
        private bool active = false;
        private Texture2D background;
        private Texture2D no;
        private Texture2D yes;
        private Texture2D pickTextOK;
        private Texture2D pickTextIcon;

        private Texture2D[] textureSource;

        private Vector2 bgPos;
        private Vector2 yesPos;
        private Vector2 noPos;
        private PickVariables noPick;
        private PickVariables yesPick;
        private ContentManager content;

        bool showIcons;
        int activeItem;
        String title;
        List<PromptItem> itemList;
        List<PickVariables> itemPick;

        private static PromptWindow instance = null;

        public static PromptWindow Inst()
        {
            if (instance == null)
            {
                instance = new PromptWindow();
            }

            return instance;
        }

        private PromptWindow()
        {
            spriteBatch = GameState.spriteBatch;
            bgPos = new Vector2(0, 0);
            active = false;

            noPick = new PickVariables(Color.Bisque);
            yesPick = new PickVariables(Color.Aquamarine);

            itemList = new List<PromptItem>();
            itemPick = new List<PickVariables>();
        }

        public void addPromptItem(PromptItem item)
        {
            itemList.Add(item);
            int size = itemList.Count;
            itemPick.Add(new PickVariables(new Color(size / 256.0f, size / 256.0f, 0.0f)));
        }
        public void showPrompt(String title, bool showIcons)
        {
            this.title = title;
            this.showIcons = showIcons;
            itemList.Clear();
            itemPick.Clear();

            active = true;
            activeItem = 0;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (content == null)
                content = new ContentManager(GameState.game.Services, "Content");

            background = content.Load<Texture2D>("HUD/WindowPromt");
            no = content.Load<Texture2D>("HUD/NOPromt");
            yes = content.Load<Texture2D>("HUD/OKPromt");
            pickTextOK = content.Load<Texture2D>("HUD/PickPromt");
            pickTextIcon = content.Load<Texture2D>("HUD/PickIcon");
            textureSource = new Texture2D[5];
            textureSource[0] = content.Load<Texture2D>("HUD/scorn");
            textureSource[1] = content.Load<Texture2D>("HUD/smeat");
            textureSource[2] = content.Load<Texture2D>("HUD/sstone");
            textureSource[3] = content.Load<Texture2D>("HUD/swood");
            textureSource[4] = content.Load<Texture2D>("HUD/sore");

            bgPos = new Vector2((Settings.maximumResolution.X - background.Width) / 2,
                                   (Settings.maximumResolution.Y - background.Height) / 2);
            int border = 50;
            yesPos = new Vector2(bgPos.X + border, bgPos.Y + background.Height - border - yes.Height);
            noPos = new Vector2(bgPos.X - no.Width - border + background.Width, bgPos.Y + background.Height - border - no.Height);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            //content.Dispose();
        }

        public override void HandlePickableAreas(Color c)
        {
            if (active)
            {
                Map.SetPickVariables(c == noPick.pickColor, noPick);
                Map.SetPickVariables(c == yesPick.pickColor, yesPick);

                for (int loop1 = 0; loop1 < itemPick.Count; loop1++)
                {
                    Map.SetPickVariables(c == itemPick[loop1].pickColor, itemPick[loop1]);
                    if (itemPick[loop1].pickNewPress)
                    {
                        activeItem = loop1;
                    }
                }

                if (yesPick.pickNewPress)
                {
                    yesPick.pickNewPress = false;
                    active = false;

                    itemList[activeItem].Execute();
                }
                else if (noPick.pickNewPress)
                {
                    noPick.pickNewPress = false;
                    active = false;
                }
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

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            
                spriteBatch.Draw(background, bgPos, color);

                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTextOK, yesPos, yesPick.pickColor);
                else
                    spriteBatch.Draw(yes, yesPos, Color.White);

                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTextOK, noPos, noPick.pickColor);
                else
                    spriteBatch.Draw(no, noPos, Color.White);

                float titleWidth = GameState.materialsNewFont.MeasureString(title).X;
                spriteBatch.DrawString(GameState.materialsNewFont, title, new Vector2(bgPos.X + (background.Width - titleWidth) / 2, bgPos.Y + 20), Color.LightBlue);

                if(showIcons)
                    DrawIcons();

                spriteBatch.DrawString(GameState.materialsNewFont, itemList[activeItem].getTitle(), new Vector2(bgPos.X + 20, bgPos.Y + 140), Color.LightBlue);

                if (!drawingPickableAreas)
                    DrawSources();

                spriteBatch.End();
            }
        }

        void DrawIcons()
        {
            if (itemList[0].getIcon() == null)
                return;

            float border = 26.0f;
            float iconsWidth = -border;
            for (int loop1 = 0; loop1 < itemList.Count; loop1++)
            {
                iconsWidth += itemList[loop1].getIcon().Width + border;
            }
            Vector2 iconPosition = new Vector2(bgPos.X + ((background.Width - iconsWidth) / 2), bgPos.Y + 60.0f);

            for (int loop1 = 0; loop1 < itemList.Count; loop1++)
            {
                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTextIcon, iconPosition, itemPick[loop1].pickColor);
                else
                {
                    spriteBatch.Draw(itemList[loop1].getIcon(), iconPosition, Color.White);
                    if (activeItem == loop1)
                    {
                        spriteBatch.Draw(GameResources.Inst().getHudTexture(HUDTexture.IconActive), iconPosition, Color.White);
                    }
                }
                iconPosition += new Vector2(itemList[loop1].getIcon().Width + 10.0f, 0.0f);
            }
        }

        void DrawSources()
        {
            
            float border = 16.0f;
            float sourcesWidth = -border;
            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (itemList[activeItem].getCost()[loop1] != 0)
                    sourcesWidth += textureSource[loop1].Width + border;
            }

            float startX = bgPos.X + ((background.Width - sourcesWidth) / 2);
            float startY = bgPos.Y + background.Height - textureSource[0].Height - 120;

            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (itemList[activeItem].getCost()[loop1] != 0)
                {
                    spriteBatch.Draw(textureSource[loop1], new Vector2(startX, startY), Color.White);
                    spriteBatch.DrawString(GameState.materialsNewFont, itemList[activeItem].getCost()[loop1].ToString(), new Vector2(startX + (textureSource[loop1].Width >> 1) - 10, startY + 65), Color.White);         
                    startX += textureSource[loop1].Width + border;
                }
            }
        }
    }
}
