using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using CorePlugin;

namespace Expanze
{
    class PlayerSettingRowComponent : GuiComponent
    {

        //space between texts in HUD of materials
        const int space = 150;
        //int start = 60;

        // active player
        private bool active = false;
        // if true, it means that this click was already catched - fix because add/rem buttons are on the same place
        private bool alreadyChanged = false;

        List<ButtonComponent> playerButtons = new List<ButtonComponent>();
        Texture2D playerColorTexture;
        Color playerColor;
        ButtonComponent playerState;
        String name = "Player";
        ButtonComponent addButton;
        ButtonComponent remButton;

        public PlayerSettingRowComponent(Game game, int x, int y, SpriteFont font, int width, int height, Color c,String name, int changeState) 
            : base(game,x,y,font,width,height,null) 
        {
            playerColorTexture = game.Content.Load<Texture2D>("pcolor");
            playerColor = c;
            this.name = name;
            playerState = new ButtonComponent(game, x + 500, y - 6, font, Settings.scaleW(200), Settings.scaleH(45), null, Settings.PlayerState);
            playerState.Actions += PlayerStateButtonAction;
            for(int loop1 = 0; loop1 < changeState; loop1++)
                playerState.nextText();

            playerState.Initialize(); playerState.LoadContent();
            addButton = new ButtonComponent(game, x, y, new Rectangle(), font, 34, 32, "HUD/hotseat_plus");
            addButton.Actions += AddButtonAction;
            addButton.Initialize(); addButton.LoadContent();
            remButton = new ButtonComponent(game, x, y, new Rectangle(), font, 34, 32, "HUD/hotseat_minus");
            remButton.Actions += RemButtonAction;
            remButton.Initialize(); remButton.LoadContent();
        }

        public string GetName() { return name; }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            playerState.Update(gameTime);
            addButton.Update(gameTime);
            remButton.Update(gameTime);
        }

        public void setIndexOfText(int index)
        {
            playerState.setActiveTextIndex(index);
        }


        public Player getPlayerSettings()
        {
            if (active)
            {
                if (Strings.Inst().GetString(TextEnum.MENU_HOT_SEAT_NO_AI) == playerState.getSelectedState())
                    return new Player(name, playerColor, null, GameMaster.Inst().GetPlayerCount());
                else
                {
                    IComponentAI componentAI = null;
                    foreach (IComponentAI AI in CoreProviderAI.AI)
                    {
                        if (AI.GetAIName().CompareTo(playerState.getSelectedState()) == 0)
                        {
                            componentAI = AI;
                            break;
                        }
                    }
                    IComponentAI componentAICopy = componentAI.Clone();
                    return new Player(name, playerColor, componentAICopy, GameMaster.Inst().GetPlayerCount());
                }
            }

            return null;
        }

        public void setActive(bool a)
        {
            this.active = a;
        }

        public bool isActive()
        {
            return this.active;
        }

        /// <summary>
        /// Event handler for remove button
        /// </summary>
        void RemButtonAction(object sender, PlayerIndexEventArgs e)
        {
            if (active && !alreadyChanged)
            {
                this.active = false;
                alreadyChanged = true;
            }
            else
            {
                alreadyChanged = false;
            }
        }

        /// <summary>
        /// Event handler for player state button
        /// </summary>
        void PlayerStateButtonAction(object sender, PlayerIndexEventArgs e)
        {
            if (active )
            {
                playerState.changeText();
            }
        }

        /// <summary>
        /// Event handler for add button
        /// </summary>
        void AddButtonAction(object sender, PlayerIndexEventArgs e)
        {
            if (!active && !alreadyChanged)
            {
                this.active = true;
                alreadyChanged = true;
            }
            else
            {
                alreadyChanged = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Color c;
            if (active)
                c = Color.BurlyWood;
            else
                c = Color.Gray;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);

            if (pick)
            {
                spriteBatch.End();
                return;
            }

            if (active)
            {
                remButton.Draw(gameTime);
            }
            else
            {
                addButton.Draw(gameTime);
            }

            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.PlayerNameFont), name, new Vector2(spritePosition.X + 80, spritePosition.Y - 6), c);
            spriteBatch.Draw(playerColorTexture, new Vector2(spritePosition.X + 420, spritePosition.Y), playerColor);

            if( active )
                playerState.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
