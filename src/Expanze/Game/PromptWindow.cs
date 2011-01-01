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
    class PromptWindow : GameComponent
    {
        public enum Mod { Viewer, Buyer };

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

        Mod mod;
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

        public bool getIsActive()
        {
            return active;
        }

        public void setIsActive(bool active)
        {
            this.active = active;
        }

        public void AddPromptItem(PromptItem item)
        {
            itemList.Add(item);
            int size = itemList.Count;
            itemPick.Add(new PickVariables(new Color(size / 256.0f, size / 256.0f, 0.0f)));
        }
        public void Show(Mod mod, String title, bool showIcons)
        {
            this.mod = mod;
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
            int borderX = 190;
            int borderY = 20;
            yesPos = new Vector2(bgPos.X + borderX, bgPos.Y + background.Height - borderY - yes.Height);
            noPos = new Vector2(bgPos.X - no.Width - borderX + background.Width, bgPos.Y + background.Height - borderY - no.Height);
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

                if (yesPick.pickNewPress || GameState.CurrentKeyboardState.IsKeyDown(Keys.Enter))
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

                spriteBatch.Begin(SpriteSortMode.Deferred, (drawingPickableAreas) ? BlendState.Opaque : BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            
                spriteBatch.Draw(background, bgPos, color);

                if (itemList[activeItem].TryExecute() == null && mod == Mod.Buyer) // it means that it is ok
                {
                    if (drawingPickableAreas)
                        spriteBatch.Draw(pickTextOK, yesPos, yesPick.pickColor);
                    else
                        spriteBatch.Draw(yes, yesPos, Color.White);
                }

                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTextOK, noPos, noPick.pickColor);
                else
                    spriteBatch.Draw(no, noPos, Color.White);

                float titleWidth = GameState.medievalBig.MeasureString(title).X;
                spriteBatch.DrawString(GameState.medievalBig, title, new Vector2(bgPos.X + (background.Width - titleWidth) / 2, bgPos.Y + 20), Color.LightBlue);

                if(showIcons)
                    DrawIcons();

                spriteBatch.DrawString(GameState.medievalMedium, itemList[activeItem].getTitle(), new Vector2(bgPos.X + 20, bgPos.Y + 160), Color.LightBlue);
                TextWrapping.DrawStringIntoRectangle(itemList[activeItem].getDescription(),
                    GameState.medievalSmall, Color.LightSteelBlue, bgPos.X + 20, bgPos.Y + 195, background.Width - 40);

                String error = itemList[activeItem].TryExecute();
                if (mod == Mod.Viewer)
                    error = Strings.ALERT_TITLE_THIS_IS_NOT_YOURS;
                if (error != null)
                {
                    TextWrapping.DrawStringIntoRectangle(error,
                     GameState.medievalSmall, Color.DarkSlateGray, bgPos.X + 22, bgPos.Y + 267, background.Width - 40);
                    TextWrapping.DrawStringIntoRectangle(error,
                     GameState.medievalSmall, Color.Red, bgPos.X + 20, bgPos.Y + 265, background.Width - 40);
                }
                if (!drawingPickableAreas)
                    DrawSources();

                spriteBatch.End();
            }
        }

        void DrawIcons()
        {
            if (itemList[0].getIcon() == null)
                return;

            float border = 15.0f;
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
                    itemList[loop1].DrawIcon(iconPosition);
                    if (activeItem == loop1 && itemList.Count > 1)
                    {
                        spriteBatch.Draw(GameResources.Inst().GetHudTexture(HUDTexture.IconActive), iconPosition, Color.White);
                    }
                }
                iconPosition += new Vector2(itemList[loop1].getIcon().Width + border, 0.0f);
            }
        }

        void DrawSources()
        {
            
            float border = 16.0f;
            float sourcesWidth = -border;
            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (itemList[activeItem].getCost()[loop1] != 0 ||
                    itemList[activeItem].getShowZeroSources())
                    sourcesWidth += textureSource[loop1].Width + border;
            }

            float startX = bgPos.X + ((background.Width - sourcesWidth) / 2);
            float startY = bgPos.Y + background.Height - textureSource[0].Height - 85;

            ISourceAll playerSource = GameMaster.Inst().GetActivePlayer().GetSource();
            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                if (itemList[activeItem].getCost()[loop1] != 0 ||
                    itemList[activeItem].getShowZeroSources())
                {
                    spriteBatch.Draw(textureSource[loop1], new Vector2(startX, startY), Color.White);
                    spriteBatch.DrawString(GameState.materialsNewFont, itemList[activeItem].getCost()[loop1].ToString(), new Vector2(startX + (textureSource[loop1].Width >> 1) + 5, startY + 45), (!itemList[activeItem].getIsSourceCost() || playerSource[loop1] >= itemList[activeItem].getCost()[loop1]) ? Color.White : Color.DarkRed);         
                    startX += textureSource[loop1].Width + border;
                }
            }
        }
    }
}
