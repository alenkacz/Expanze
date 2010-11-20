using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze
{
    class MarketComponent : GuiComponent
    {
        #region Properties
        public static bool isActive = false;
        private static MarketComponent instance = null;

        //list of to and from buttons - top and bottom row
        List<ButtonComponent> fromButtons = new List<ButtonComponent>();
        List<ButtonComponent> toButtons = new List<ButtonComponent>();
        List<GuiComponent> content = new List<GuiComponent>();

        //size of the button
        Vector2 buttonSize = new Vector2(120, 120);
        //space between buttons
        int space = 20;
        Rectangle range;

        #endregion

        #region Initialization
        private MarketComponent()

            : base(Settings.Game, 200, (int)Settings.maximumResolution.Y-600, GameState.gameFont, Settings.scaleW(700), Settings.scaleH(400), "market_bg")
        {
            this.range = new Rectangle(200, (int)Settings.maximumResolution.Y - 600, 700, 400);
        }

        public static MarketComponent getInstance()
        {
            if (instance == null)
            {
                instance = new MarketComponent();
            }

            return instance;
        }

        public override void Initialize()
        {
            base.Initialize();

            this.createFirstRow();
            this.createSecondRow();

            ButtonComponent change_button = new ButtonComponent(Settings.Game, range.Right - 200, (int)(range.Bottom-50), new Rectangle(), GameState.gameFont, Settings.scaleW(160), Settings.scaleH(45), "change-button");
            change_button.Actions += ChangeButtonAction;
            this.content.Add(change_button);

            ButtonComponent exit_button = new ButtonComponent(Settings.Game, range.Right-42, (int)(range.Top + Settings.scaleH(29)), new Rectangle(), GameState.gameFont, Settings.scaleW(42), Settings.scaleH(29), "close");
            exit_button.Actions += CloseButtonAction;
            this.content.Add(exit_button);

            foreach (GuiComponent g in content)
            {
                g.Initialize();
                g.LoadContent();
            }
        }

        /// <summary>
        /// Creates first row of the buttons on a market - what to change
        /// </summary>
        protected void createFirstRow()
        {
            //top row
            ButtonComponent corn_button = new ButtonComponent(Settings.Game, range.Left + 2 * space, (int)(range.Top + space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "obili-button", HexaKind.Cornfield);
            corn_button.Actions += MaterialButtonAction;
            this.content.Add(corn_button);
            ButtonComponent brick_button = new ButtonComponent(Settings.Game, (int)(range.Left + buttonSize.X + space), (int)(range.Top + space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "cihla-button", HexaKind.Mountains);
            brick_button.Actions += MaterialButtonAction;
            this.content.Add(brick_button);
            ButtonComponent wood_button = new ButtonComponent(Settings.Game, (int)(range.Left + 2 * buttonSize.X + space), (int)(range.Top + space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "drevo-button", HexaKind.Forest);
            wood_button.Actions += MaterialButtonAction;
            this.content.Add(wood_button);
            ButtonComponent stone_button = new ButtonComponent(Settings.Game, (int)(range.Left + 3 * buttonSize.X + space), (int)(range.Top + space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "kamen-button", HexaKind.Stone);
            stone_button.Actions += MaterialButtonAction;
            this.content.Add(stone_button);
            ButtonComponent sheep_button = new ButtonComponent(Settings.Game, (int)(range.Left + 4 * buttonSize.X + space), (int)(range.Top + space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "ovce-button", HexaKind.Pasture);
            sheep_button.Actions += MaterialButtonAction;
            this.content.Add(sheep_button);

            toButtons.Add(corn_button);
            toButtons.Add(brick_button);
            toButtons.Add(wood_button);
            toButtons.Add(stone_button);
            toButtons.Add(sheep_button);
        }

        /// <summary>
        /// Creates second row of buttons - what material are we offering to trade
        /// </summary>
        protected void createSecondRow()
        {
            //bottom row
            ButtonComponent corn2_button = new ButtonComponent(Settings.Game, range.Left + 2 * space, (int)(range.Top + buttonSize.Y + 2 * space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "obili-button",HexaKind.Cornfield);
            corn2_button.Actions += MaterialButtonAction;
            this.content.Add(corn2_button);
            ButtonComponent brick2_button = new ButtonComponent(Settings.Game, (int)(range.Left + buttonSize.X + space), (int)(range.Top + buttonSize.Y + 2 * space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "cihla-button",HexaKind.Mountains);
            brick2_button.Actions += MaterialButtonAction;
            this.content.Add(brick2_button);
            ButtonComponent wood2_button = new ButtonComponent(Settings.Game, (int)(range.Left + 2 * buttonSize.X + space), (int)(range.Top + buttonSize.Y + 2 * space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "drevo-button",HexaKind.Forest);
            wood2_button.Actions += MaterialButtonAction;
            this.content.Add(wood2_button);
            ButtonComponent stone2_button = new ButtonComponent(Settings.Game, (int)(range.Left + 3 * buttonSize.X + space), (int)(range.Top + buttonSize.Y + 2 * space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "kamen-button",HexaKind.Stone);
            stone2_button.Actions += MaterialButtonAction;
            this.content.Add(stone2_button);
            ButtonComponent sheep2_button = new ButtonComponent(Settings.Game, (int)(range.Left + 4 * buttonSize.X + space), (int)(range.Top + buttonSize.Y + 2 * space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "ovce-button", HexaKind.Pasture);
            sheep2_button.Actions += MaterialButtonAction;
            this.content.Add(sheep2_button);

            fromButtons.Add(corn2_button);
            fromButtons.Add(brick2_button);
            fromButtons.Add(wood2_button);
            fromButtons.Add(stone2_button);
            fromButtons.Add(sheep2_button);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            foreach (GuiComponent g in content)
            {
                g.UnloadContent();
            }
        }
        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Event handler for materials button
        /// </summary>
        void MaterialButtonAction(object sender, PlayerIndexEventArgs e)
        {
            ButtonComponent btn = (ButtonComponent)sender;

            //reset - only one button from the row can be picked
            if (toButtons.Contains(btn))
            {
                foreach (ButtonComponent b in toButtons)
                {
                    b.setPicked(false);
                }
            }
            else
            {
                //dealing with second row of the buttons
                foreach (ButtonComponent b in fromButtons)
                {
                    b.setPicked(false);
                }
            }

            btn.setPicked(!btn.getPicked());
        }

        /// <summary>
        /// Event handler for change button
        /// </summary>
        void ChangeButtonAction(object sender, PlayerIndexEventArgs e)
        {
            ButtonComponent btn = (ButtonComponent)sender;
            ButtonComponent selectedFrom = null;
            ButtonComponent selectedTo = null;

            foreach (ButtonComponent b in fromButtons)
            {
                if (b.getPicked()) { selectedFrom = b; break; }
            }

            foreach (ButtonComponent b in toButtons)
            {
                if (b.getPicked()) { selectedTo = b; break; }
            }

            HexaKind fromType = HexaKind.Null;
            HexaKind toType = HexaKind.Null;

            if (selectedTo != null && selectedFrom != null)
            {
                fromType = selectedFrom.getType();
                toType = selectedTo.getType();
            }
            else
            {
                WindowPromt alert = new WindowPromt();
                alert.showAlert("You must choose from both rows of buttons");
            }

            if (fromType != HexaKind.Null && toType != HexaKind.Null)
            {
                GameMaster.getInstance().doMaterialConversion(fromType, toType, GameMaster.getInstance().getActivePlayer());
            }

        }

        /// <summary>
        /// Event handler for close button
        /// </summary>
        void CloseButtonAction(object sender, PlayerIndexEventArgs e)
        {
            MarketComponent.isActive = false;
        }

        #endregion

        #region Update and Draw
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (GuiComponent g in content)
            {
                g.Update(gameTime);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (GuiComponent g in content)
            {
                g.Draw(gameTime);
            }
        }

        #endregion
    }
}
