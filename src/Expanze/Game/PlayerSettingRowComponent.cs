using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Expanze
{
    class PlayerSettingRowComponent : GuiComponent
    {

        //space between texts in HUD of materials
        const int space = 150;
        int start = 60;

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

        public PlayerSettingRowComponent(Game game, int x, int y, SpriteFont font, int width, int height, Color c,String name) 
            : base(game,x,y,font,width,height,null) 
        {
            playerColorTexture = game.Content.Load<Texture2D>("pcolor");
            playerColor = c;
            this.name = name;
            playerState = new ButtonComponent(game, x + 600, y, font, 200, 50, null, Settings.PlayerState);
            playerState.Initialize(); playerState.LoadContent();
            addButton = new ButtonComponent(game, x, y, new Rectangle(), font, 104, 45, "HUD/OKPromt");
            addButton.Actions += AddButtonAction;
            addButton.Initialize(); addButton.LoadContent();
            remButton = new ButtonComponent(game, x, y, new Rectangle(), font, 104, 45, "HUD/NOPromt");
            remButton.Actions += RemButtonAction;
            remButton.Initialize(); remButton.LoadContent();
        }

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
                if ("Hráč" == playerState.getSelectedState())
                    return new Player(name, playerColor, null);
                else
                    return null;
                //TODO start AI game
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
                c = Color.White;
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

            spriteBatch.DrawString(GameState.playerNameFont, name, new Vector2(spritePosition.X + 200, spritePosition.Y), c);
            spriteBatch.Draw(playerColorTexture, new Vector2(spritePosition.X + 500, spritePosition.Y), playerColor);

            if( active )
                playerState.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
