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

        Mod mod;
        bool showIcons;
        int activeItem;
        int hoverItem;
        bool hoverYes;
        bool hoverNo;
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

        public int GetActiveItem() { return activeItem; }

        public void SetActiveItem(int index)
        {
            activeItem = index;
            if (activeItem < 0)
                activeItem = 0;
            if (activeItem >= GetItemCount())
                activeItem = GetItemCount() - 1;
        }

        public bool GetIsActive()
        {
            return active;
        }

        public void Deactive()
        {
            active = false;
            InputManager.Inst().ClearActiveState("gamewindow");
        }

        public void AddPromptItem(PromptItem item)
        {
            itemList.Add(item);
            int size = itemList.Count;
            itemPick.Add(new PickVariables(new Color(size / 16.0f, size / 16.0f, 0.0f)));
        }

        public int GetItemCount() { return itemList.Count; }

        public void Show(Mod mod, String title, bool showIcons)
        {
            if (Message.Inst().GetIsActive())
                return;
            this.mod = mod;
            this.title = title;
            this.showIcons = showIcons;
            itemList.Clear();
            itemPick.Clear();

            active = true;
            activeItem = 0;
            hoverItem = -1;
            hoverNo = false;
            hoverYes = false;

            InputManager.Inst().SetActiveState("gamewindow");
        }

        public override void LoadContent()
        {
            base.LoadContent();

            background = GameResources.Inst().GetHudTexture(HUDTexture.BackgroundPromptWindow);
            no = GameResources.Inst().GetHudTexture(HUDTexture.ButtonNo);
            yes = GameResources.Inst().GetHudTexture(HUDTexture.ButtonYes);
            pickTextOK = GameResources.Inst().GetHudTexture(HUDTexture.PickWindowPrompt);
            pickTextIcon = GameResources.Inst().GetHudTexture(HUDTexture.PickWindowIcon);
            textureSource = new Texture2D[5];
            textureSource[0] = GameResources.Inst().GetHudTexture(HUDTexture.SmallCorn);
            textureSource[1] = GameResources.Inst().GetHudTexture(HUDTexture.SmallMeat);
            textureSource[2] = GameResources.Inst().GetHudTexture(HUDTexture.SmallStone);
            textureSource[3] = GameResources.Inst().GetHudTexture(HUDTexture.SmallWood);
            textureSource[4] = GameResources.Inst().GetHudTexture(HUDTexture.SmallOre);

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
                hoverItem = -1;
                hoverYes = false;
                hoverNo = false;

                Map.SetPickVariables(c == noPick.pickColor, noPick);
                Map.SetPickVariables(c == yesPick.pickColor, yesPick);

                for (int loop1 = 0; loop1 < itemPick.Count; loop1++)
                {
                    Map.SetPickVariables(c == itemPick[loop1].pickColor, itemPick[loop1]);
                    if (itemPick[loop1].pickNewPress)
                    {
                        activeItem = loop1;
                    }

                    if (itemPick[loop1].pickActive)
                        hoverItem = loop1;
                }

                if (mod == Mod.Buyer && InputManager.Inst().GetGameAction("gamewindow", "changesources").IsPressed() && MarketComponent.Inst().IsOpen)
                {
                    GameMaster.Inst().ChangeSourcesFor((SourceAll) itemList[activeItem].getCost());
                }

                if (mod == Mod.Buyer && InputManager.Inst().GetGameAction("gamewindow", "canchange").IsPressed())
                {
                    int amount = GameMaster.Inst().CanChangeSourcesFor((SourceAll)itemList[activeItem].getCost());

                    if (amount < 0)
                        Message.Inst().Show(Strings.Inst().GetString(TextEnum.MESSAGE_TITLE_MARKET_NOT_SOURCES), Strings.Inst().GetString(TextEnum.MESSAGE_DESCRIPTION_MARKET_NOT_SOURCES), GameResources.Inst().GetHudTexture(HUDTexture.IconMarket));
                    else if(itemList[activeItem].getCost().HasPlayerSources(GameMaster.Inst().GetActivePlayer()))
                        Message.Inst().Show(Strings.Inst().GetString(TextEnum.MESSAGE_TITLE_MARKET_BUY_IT), Strings.Inst().GetString(TextEnum.MESSAGE_DESCRIPTION_MARKET_BUY_IT), GameResources.Inst().GetHudTexture(HUDTexture.IconMarket));
                    else
                        Message.Inst().Show(Strings.Inst().GetString(TextEnum.MESSAGE_TITLE_MARKET_CHANGE_SOURCES), Strings.Inst().GetString(TextEnum.MESSAGE_DESCRIPTION_MARKET_CHANGE_SOURCES), GameResources.Inst().GetHudTexture(HUDTexture.IconMarket));
                }

                if (InputManager.Inst().GetGameAction("gamewindow", "left").IsPressed())
                {
                    activeItem--;
                    if (activeItem < 0)
                        activeItem = 0;
                }

                if (InputManager.Inst().GetGameAction("gamewindow", "right").IsPressed())
                {
                    activeItem++;
                    if (activeItem >= itemList.Count)
                        activeItem = itemList.Count - 1;
                }

                if (yesPick.pickActive)
                    hoverYes = true;
                else if (noPick.pickActive)
                    hoverNo = true;


                if (yesPick.pickNewPress || InputManager.Inst().GetGameAction("gamewindow", "confirm").IsPressed())
                {
                    yesPick.pickNewPress = false;
                    Deactive();

                    itemList[activeItem].Execute();
                }
                else if (noPick.pickNewPress || InputManager.Inst().GetGameAction("gamewindow", "close").IsPressed())
                {
                    noPick.pickNewPress = false;
                    Deactive();
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

                if (itemList[activeItem].TryExecute() == null && mod == Mod.Buyer) // it means that it is ok
                {
                    if (drawingPickableAreas)
                        spriteBatch.Draw(pickTextOK, yesPos, yesPick.pickColor);
                    else
                        spriteBatch.Draw(yes, yesPos, hoverYes ? Settings.colorHoverItem : Settings.colorPassiveItem);
                }

                if (drawingPickableAreas)
                    spriteBatch.Draw(pickTextOK, noPos, noPick.pickColor);
                else
                    spriteBatch.Draw(no, noPos, hoverNo ? Settings.colorHoverItem : Settings.colorPassiveItem);

                if (!drawingPickableAreas)
                {
                    float titleWidth = GameResources.Inst().GetFont(EFont.MedievalBig).MeasureString(title).X;
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), title, new Vector2(bgPos.X + (background.Width - titleWidth) / 2, bgPos.Y + 20), Color.LightBlue);
                }

                if(showIcons)
                    DrawIcons();

                if (!drawingPickableAreas)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalMedium), itemList[activeItem].getTitle(), new Vector2(bgPos.X + 20, bgPos.Y + 160), Color.LightBlue);
                    TextWrapping.DrawStringIntoRectangle(itemList[activeItem].getDescription(),
                        GameResources.Inst().GetFont(EFont.MedievalSmall), Color.LightSteelBlue, bgPos.X + 20, bgPos.Y + 195, background.Width - 40);
                }

                String error = itemList[activeItem].TryExecute();
                if (mod == Mod.Viewer)
                    error = Strings.Inst().GetString(TextEnum.ALERT_TITLE_THIS_IS_NOT_YOURS);
                int errorY = 265;
                if (itemList[activeItem].getCost().Equals(new SourceAll(0)))
                    errorY = 300;
                if (error != null && !drawingPickableAreas)
                {
                    TextWrapping.DrawStringIntoRectangle(error,
                     GameResources.Inst().GetFont(EFont.MedievalSmall), Color.DarkSlateGray, bgPos.X + 22, bgPos.Y + errorY + 2, background.Width - 40);
                    TextWrapping.DrawStringIntoRectangle(error,
                     GameResources.Inst().GetFont(EFont.MedievalSmall), Color.Red, bgPos.X + 20, bgPos.Y + errorY, background.Width - 40);
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
                    itemList[loop1].DrawIcon(iconPosition, hoverItem == loop1 && activeItem != loop1);
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
            GameMaster.Inst().GetActivePlayer().SetMaterialChange(-((SourceAll) itemList[activeItem].getCost()));
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
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), itemList[activeItem].getCost()[loop1].ToString(), new Vector2(startX + (textureSource[loop1].Width >> 1) + 5, startY + 45), (!itemList[activeItem].getIsSourceCost() || playerSource[loop1] >= itemList[activeItem].getCost()[loop1]) ? Color.White : Color.DarkRed);         
                    startX += textureSource[loop1].Width + border;
                }
            }
        }
    }
}
