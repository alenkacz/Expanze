using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class PlayerSettingRowComponent : GuiComponent
    {

        //space between texts in HUD of materials
        const int space = 150;
        int start = 60;

        List<ButtonComponent> playerButtons = new List<ButtonComponent>();
        Texture2D playerColorTexture;
        Color playerColor;
        ButtonComponent playerState;

        public PlayerSettingRowComponent(Game game, int x, int y, SpriteFont font, int width, int height, Color c) 
            : base(game,x,y,font,width,height,null) 
        {
            playerColorTexture = game.Content.Load<Texture2D>("pcolor");
            playerColor = c;
            playerState = new ButtonComponent(game, x + 300, y, font, 200, 50, null, Settings.PlayerState);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            playerState.Update(gameTime);
        }

        public void setIndexOfText(int index)
        {
            playerState.setActiveTextIndex(index);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Color c;
            if (pick)
                c = Color.Black;
            else
                c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);

            if (pick)
            {
                spriteBatch.End();
                return;
            }

            spriteBatch.Draw(playerColorTexture, spritePosition, playerColor);
            spriteBatch.DrawString(GameState.playerNameFont, "Player", new Vector2(spritePosition.X + 100, spritePosition.Y), Color.White);
            playerState.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
