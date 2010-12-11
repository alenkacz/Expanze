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
        private bool wasActive = false;
        private static MarketComponent instance = null;

        //list of to and from buttons - top and bottom row
        List<ButtonComponent> fromButtons = new List<ButtonComponent>();
        List<ButtonComponent> toButtons = new List<ButtonComponent>();
        List<GuiComponent> content = new List<GuiComponent>();

        MarketSliderComponent marketSlider;

        //size of the button
        Vector2 buttonSize = new Vector2(57, 60);
        //space between buttons
        int space = 50;
        int w_space = 20;
        int leftMargin = 170;
        int topMargin = 40;
        Rectangle range;

        HexaKind toSelectKind = HexaKind.Null;
        HexaKind fromSelectKind = HexaKind.Null;

        Dictionary<HexaKind,Texture2D> marketKindsTextures = new Dictionary<HexaKind,Texture2D>();
        Vector2 toIconPosition;
        Vector2 fromIconPosition;

        #endregion

        #region Initialization
        private MarketComponent()

            : base(Settings.Game, 200, (int)Settings.maximumResolution.Y-600, GameState.gameFont, Settings.scaleW(658), Settings.scaleH(446), "market-bg")
        {
            this.range = new Rectangle(200, (int)Settings.maximumResolution.Y - 600, 658, 446);

            toIconPosition = new Vector2(this.range.Right - 100, (int)(range.Top + topMargin + buttonSize.Y + 3 * space + 30));
            fromIconPosition = new Vector2(this.range.Left + 30, (int)(range.Top + topMargin + buttonSize.Y + 3 * space + 30));
        }

        public static MarketComponent getInstance()
        {
            if (instance == null)
            {
                instance = new MarketComponent();
            }

            return instance;
        }

        public bool getIsActive()
        {
            return isActive;
        }

        public void setIsActive(bool active)
        {
            isActive = active;
        }

        public override void Initialize()
        {
            base.Initialize();

            this.createFirstRow();
            this.createSecondRow();

            fillMarketKindTextures();

            ButtonComponent change_button = new ButtonComponent(Settings.Game, range.Left + 180, (int)(range.Bottom-80), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(45), "HUD/OKPromt");
            change_button.Actions += ChangeButtonAction;
            this.content.Add(change_button);

            ButtonComponent exit_button = new ButtonComponent(Settings.Game, range.Left + 400, (int)(range.Bottom - 80), new Rectangle(), GameState.gameFont, Settings.scaleW(104), Settings.scaleH(46), "HUD/NOPromt");
            exit_button.Actions += CloseButtonAction;
            this.content.Add(exit_button);

            GuiComponent title = new GuiComponent(Settings.Game, range.Left + 240, (int)(range.Top + 20), GameState.gameFont, Settings.scaleW(217), Settings.scaleH(43), "trziste_nadpis");
            this.content.Add(title);

            GuiComponent firstRow = new GuiComponent(Settings.Game, range.Left + 3*w_space, (int)(range.Top + topMargin + space + 10), GameState.gameFont, Settings.scaleW(154), Settings.scaleH(22), "co_vymenit");
            this.content.Add(firstRow);

            GuiComponent secondRow = new GuiComponent(Settings.Game, range.Left + 3 * w_space, (int)(range.Top + topMargin + buttonSize.Y + 2 * space + 10), GameState.gameFont, Settings.scaleW(77), Settings.scaleH(14), "za_co_vymenit");
            this.content.Add(secondRow);

            marketSlider = new MarketSliderComponent(Settings.Game, range.Left + 130, (int)(range.Top + topMargin + buttonSize.Y + 4 * space + 10), GameState.gameFont,392,16,"slider_market");
            //this.content.Add(marketSlider);

            foreach (GuiComponent g in content)
            {
                g.Initialize();
                g.LoadContent();
            }

            marketSlider.Initialize(); marketSlider.LoadContent();
        }

        private void fillMarketKindTextures()
        {
            marketKindsTextures = new Dictionary<HexaKind, Texture2D>();
            marketKindsTextures.Add(HexaKind.Cornfield,Settings.Game.Content.Load<Texture2D>("corn_market"));
            marketKindsTextures.Add(HexaKind.Stone, Settings.Game.Content.Load<Texture2D>("stone_market"));
            marketKindsTextures.Add(HexaKind.Mountains, Settings.Game.Content.Load<Texture2D>("ore_market"));
            marketKindsTextures.Add(HexaKind.Pasture, Settings.Game.Content.Load<Texture2D>("meat_market"));
            marketKindsTextures.Add(HexaKind.Forest, Settings.Game.Content.Load<Texture2D>("wood_market"));
        }

        /// <summary>
        /// Creates first row of the buttons on a market - what to change
        /// </summary>
        protected void createFirstRow()
        {
            //top row
            ButtonComponent corn_button = new ButtonComponent(Settings.Game, leftMargin + range.Left + 3 * w_space, (int)(range.Top + topMargin + space), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "corn_market", "corn_market_nonactive", HexaKind.Cornfield);
            corn_button.Actions += MaterialButtonAction;
            this.content.Add(corn_button);
            ButtonComponent brick_button = new ButtonComponent(Settings.Game, leftMargin + (int)(range.Left + buttonSize.X + 4 * w_space), (int)(range.Top + space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "stone_market", "stone_market_nonactive", HexaKind.Stone);
            brick_button.Actions += MaterialButtonAction;
            this.content.Add(brick_button);
            ButtonComponent wood_button = new ButtonComponent(Settings.Game, leftMargin + (int)(range.Left + 2 * buttonSize.X + 5 * w_space), (int)(range.Top + space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "wood_market", "wood_market_nonactive", HexaKind.Forest);
            wood_button.Actions += MaterialButtonAction;
            this.content.Add(wood_button);
            ButtonComponent stone_button = new ButtonComponent(Settings.Game, leftMargin + (int)(range.Left + 3 * buttonSize.X + 6 * w_space), (int)(range.Top + space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "ore_market", "ore_market_nonactive", HexaKind.Mountains);
            stone_button.Actions += MaterialButtonAction;
            this.content.Add(stone_button);
            ButtonComponent sheep_button = new ButtonComponent(Settings.Game, leftMargin + (int)(range.Left + 4 * buttonSize.X + 7 * w_space), (int)(range.Top + space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "meat_market", "meat_market_nonactive", HexaKind.Pasture);
            sheep_button.Actions += MaterialButtonAction;
            this.content.Add(sheep_button);

            fromButtons.Add(corn_button);
            fromButtons.Add(brick_button);
            fromButtons.Add(wood_button);
            fromButtons.Add(stone_button);
            fromButtons.Add(sheep_button);
        }

        /// <summary>
        /// Creates second row of buttons - what material are we offering to trade
        /// </summary>
        protected void createSecondRow()
        {
            //bottom row
            ButtonComponent corn_button = new ButtonComponent(Settings.Game, leftMargin + range.Left + 3*w_space, (int)(range.Top + buttonSize.Y + 2 * space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "corn_market", HexaKind.Cornfield);
            corn_button.Actions += MaterialButtonAction;
            this.content.Add(corn_button);
            ButtonComponent brick_button = new ButtonComponent(Settings.Game, leftMargin + (int)(range.Left + buttonSize.X + 4 * w_space), (int)(range.Top + buttonSize.Y + 2 * space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "stone_market", HexaKind.Stone);
            brick_button.Actions += MaterialButtonAction;
            this.content.Add(brick_button);
            ButtonComponent wood_button = new ButtonComponent(Settings.Game, leftMargin + (int)(range.Left + 2 * buttonSize.X + 5 * w_space), (int)(range.Top + buttonSize.Y + 2 * space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "wood_market", HexaKind.Forest);
            wood_button.Actions += MaterialButtonAction;
            this.content.Add(wood_button);
            ButtonComponent stone_button = new ButtonComponent(Settings.Game, leftMargin + (int)(range.Left + 3 * buttonSize.X + 6 * w_space), (int)(range.Top + buttonSize.Y + 2 * space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "ore_market", HexaKind.Mountains);
            stone_button.Actions += MaterialButtonAction;
            this.content.Add(stone_button);
            ButtonComponent sheep_button = new ButtonComponent(Settings.Game, leftMargin + (int)(range.Left + 4 * buttonSize.X + 7 * w_space), (int)(range.Top + buttonSize.Y + 2 * space + topMargin), new Rectangle(), GameState.gameFont, Settings.scaleW(buttonSize.X), Settings.scaleH(buttonSize.Y), "meat_market", HexaKind.Pasture);
            sheep_button.Actions += MaterialButtonAction;
            this.content.Add(sheep_button);

            toButtons.Add(corn_button);
            toButtons.Add(brick_button);
            toButtons.Add(wood_button);
            toButtons.Add(stone_button);
            toButtons.Add(sheep_button);
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

            if (disallowChangingSameTypes(btn))
            {
                return;
            }

            //disallow clicking on inactive button - only for first row
            if (fromButtons.Contains(btn) && !btn.isActive())
            {
                return;
            }

            //reset - only one button from the row can be picked
            if (toButtons.Contains(btn))
            {

                foreach (ButtonComponent b in toButtons)
                {
                    b.setPicked(false);
                }

                toSelectKind = btn.getType();
                marketSlider.setToType(toSelectKind);
            }
            else
            {
                //dealing with second row of the buttons
                foreach (ButtonComponent b in fromButtons)
                {
                    b.setPicked(false);
                }

                fromSelectKind = btn.getType();
                marketSlider.setFromType(fromSelectKind);
                marketSlider.moveSliderToStart();
            }

            btn.setPicked(!btn.getPicked());
        }

        /// <summary>
        /// Method is responsible for disapproving changing same material for same material - like corn for corn
        /// </summary>
        /// <returns>true if we should disallow clicking</returns>
        bool disallowChangingSameTypes(ButtonComponent btn)
        {
            if (fromButtons.Contains(btn))
            {

                foreach (ButtonComponent b in toButtons)
                {
                    //trying to find selected button
                    if (b.getPicked())
                    {
                        if (b.getType() == btn.getType()) { 
                            // first row has bigger priority
                            b.setPicked(false);
                            toSelectKind = HexaKind.Null;
                            marketSlider.setToType(HexaKind.Null);
                            return false; 
                        }
                        //we can break because only one button can be selected
                        break;
                    }
                }
            }
            else
            {
                foreach (ButtonComponent b in fromButtons)
                {
                    //trying to find selected button
                    if (b.getPicked())
                    {
                        if (b.getType() == btn.getType()) { return true; }
                        //we can break because only one button can be selected
                        break;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Event handler for change button
        /// </summary>
        void ChangeButtonAction(object sender, PlayerIndexEventArgs e)
        {

            int actualFrom = GameMaster.getInstance().getActivePlayer().getMaterialNumber(fromSelectKind);
            int actualTo = GameMaster.getInstance().getActivePlayer().getMaterialNumber(toSelectKind);

            int convertedFrom = marketSlider.getConvertedFrom();
            int convertedTo = marketSlider.getConvertedTo();

            GameMaster.getInstance().doMaterialConversion(fromSelectKind, toSelectKind, GameMaster.getInstance().getActivePlayer(),actualFrom - convertedFrom,convertedTo-actualTo);
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

            if (wasActive != isActive)
            {
                // reset when user opens or closes market window
                foreach (ButtonComponent b in fromButtons)
                {
                    b.setPicked(false);
                }

                foreach (ButtonComponent b in toButtons)
                {
                    b.setPicked(false);
                }

                fromSelectKind = HexaKind.Null;
                toSelectKind = HexaKind.Null;
                marketSlider.resetSlider();

                wasActive = isActive;
            }

            marketSlider.Update(gameTime);
        }

        private void drawSelectedKind()
        {
            if (toSelectKind != HexaKind.Null)
            {
                spriteBatch.Draw(marketKindsTextures[toSelectKind],toIconPosition,Color.White);
                //int count = GameMaster.getInstance().getActivePlayer().getMaterialNumber(toSelectKind);
                //spriteBatch.DrawString(GameState.gameFont, count.ToString(), new Vector2(toIconPosition.X, toIconPosition.Y+Settings.scaleH(90)), Color.White);
            }

            if (fromSelectKind != HexaKind.Null)
            {
                spriteBatch.Draw(marketKindsTextures[fromSelectKind], fromIconPosition, Color.White);
                //int count = GameMaster.getInstance().getActivePlayer().getMaterialNumber(fromSelectKind);
                //spriteBatch.DrawString(GameState.gameFont, count.ToString(), new Vector2(fromIconPosition.X, fromIconPosition.Y + Settings.scaleH(90)), Color.White);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);

            if (!pick)
            {
                foreach (GuiComponent g in content)
                {
                    g.Draw(gameTime);
                }

                marketSlider.Draw(gameTime);
                drawSelectedKind();
            }

            spriteBatch.End();
        }

        #endregion
    }
}
