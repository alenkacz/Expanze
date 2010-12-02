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

        public PlayerSettingRowComponent(Game game, int x, int y, SpriteFont font, int width, int height, Color c) 
            : base(game,x,y,font,width,height,null) 
        {
            playerColorTexture = game.Content.Load<Texture2D>("pcolor");
            playerColor = c;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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

            spriteBatch.Draw(playerColorTexture, spritePosition, c);
            spriteBatch.DrawString(GameState.playerNameFont, "Player", new Vector2(spritePosition.X + 100, spritePosition.Y), Color.White);

            spriteBatch.End();
        }
    }
}
