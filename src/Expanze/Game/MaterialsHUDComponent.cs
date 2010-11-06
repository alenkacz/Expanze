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

        bool changeMaterials = false;
        double materialsChangeTime = 0;

        //space between texts in HUD of materials
        const int space = 150;
        int start = 60;

        public MaterialsHUDComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture) 
            : base(game,x,y,font,width,height,texture) {}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Player act = GameMaster.getInstance().getActivePlayer();

            if (act.hasMaterialChanged())
            {
                changeMaterials = true;

            }

            if ((gameTime.ElapsedGameTime.TotalMilliseconds - materialsChangeTime) > 100)
            {
                materialsChangeTime = 0;
                changeMaterials = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);
            spritePosition.Y = 750;
            spritePosition.X = 350;

            Player act = GameMaster.getInstance().getActivePlayer();

            spriteBatch.Draw(myButton, spritePosition, Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getCorn().ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getMeat().ToString(), new Vector2(this.spritePosition.X + start + space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getStone().ToString(), new Vector2(this.spritePosition.X + start + 2 * space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getWood().ToString(), new Vector2(this.spritePosition.X + start + 3 * space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameState.hudMaterialsFont, act.getOre().ToString(), new Vector2(this.spritePosition.X + start + 4 * space, this.spritePosition.Y + 100), Color.White);

            //spriteBatch.DrawString(GameState.hudMaterialsFont, changeMaterials.ToString(), new Vector2(200, 200), Color.White);

            if (changeMaterials)
            {
                SourceCost sc = act.getMaterialChange();

                if(sc.corn != 0) {
                    spriteBatch.DrawString(GameState.materialsNewFont, sc.corn.ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 120), Color.Red);
                }

                if (sc.meat != 0)
                {
                    spriteBatch.DrawString(GameState.materialsNewFont, sc.meat.ToString(), new Vector2(this.spritePosition.X + start + space, this.spritePosition.Y + 120), Color.Red);
                }

                if (sc.ore != 0)
                {
                    spriteBatch.DrawString(GameState.materialsNewFont, sc.ore.ToString(), new Vector2(this.spritePosition.X + start + 3 * space, this.spritePosition.Y + 120), Color.Red);
                }

                if (sc.wood != 0)
                {
                    spriteBatch.DrawString(GameState.materialsNewFont, sc.wood.ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 120), Color.Red);
                }

                if (sc.stone != 0)
                {
                    spriteBatch.DrawString(GameState.materialsNewFont, sc.stone.ToString(), new Vector2(this.spritePosition.X + start + 4 * space, this.spritePosition.Y + 120), Color.Red);
                }

                if (materialsChangeTime == 0)
                {
                    materialsChangeTime = gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                changeMaterials = false;
            }

            spriteBatch.End();
        }
    }
}
