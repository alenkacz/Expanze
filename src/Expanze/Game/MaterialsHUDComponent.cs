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
        const int space = 150;
        int start = 60;

        public MaterialsHUDComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture) 
            : base(game,x,y,font,width,height,texture) {}

        public override void Draw(GameTime gameTime)
        {
            Matrix SpriteScale = Matrix.CreateScale(Settings.activeResolution.X / Settings.maximumResolution.X, Settings.activeResolution.Y / Settings.maximumResolution.Y, 1);
            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,SpriteScale);
            spritePosition.Y = 750;
            spritePosition.X = 350;

            Player act = GameMaster.getInstance().getActivePlayer();


            spriteBatch.Draw(myButton, spritePosition, Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getCorn().ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getMeat().ToString(), new Vector2(this.spritePosition.X + start + space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getStone().ToString(), new Vector2(this.spritePosition.X + start + 2 * space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getWood().ToString(), new Vector2(this.spritePosition.X + start + 3 * space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getOre().ToString(), new Vector2(this.spritePosition.X + start + 4 * space, this.spritePosition.Y + 100), Color.White);

            spriteBatch.End();
        }
    }
}
