using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class MaterialsHUDComponent : GuiComponent
    {

        //space between texts in HUD of materials
        const int space = 90;
        int start = 40;

        public MaterialsHUDComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture) 
            : base(game,x,y,font,width,height,texture) {}

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();

            Player act = GameMaster.getInstance().getActivePlayer();


            spriteBatch.Draw(myButton, spritePosition, Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getCorn().ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 55), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getMeat().ToString(), new Vector2(this.spritePosition.X + start + space, this.spritePosition.Y + 55), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getStone().ToString(), new Vector2(this.spritePosition.X + start + 2 * space, this.spritePosition.Y + 55), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getWood().ToString(), new Vector2(this.spritePosition.X + start + 3 * space, this.spritePosition.Y + 55), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getOre().ToString(), new Vector2(this.spritePosition.X + start + 4 * space, this.spritePosition.Y + 55), Color.White);

            spriteBatch.End();
        }
    }
}
