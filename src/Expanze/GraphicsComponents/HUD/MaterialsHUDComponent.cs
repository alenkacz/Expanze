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

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Player act = GameMaster.Inst().LastHumanPlayer;

            if (act.HasMaterialChanged())
            {
                changeMaterials = true;
                materialsChangeTime = 1500;
            }

            if (changeMaterials)
            {
                materialsChangeTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if (materialsChangeTime < 0)
                {
                    materialsChangeTime = 0;
                    changeMaterials = false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Color c;
            if (pick)
                c = new Color(0, 0, 0, 255);
            else
                c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);
            spritePosition.Y = 755;
            spritePosition.X = 350;

            Player act = GameMaster.Inst().LastHumanPlayer;

            spriteBatch.Draw(myButton, spritePosition, c);
            if (pick)
            {
                spriteBatch.End();
                return;
            }

            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.GetCorn().ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.GetMeat().ToString(), new Vector2(this.spritePosition.X + start + space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.GetStone().ToString(), new Vector2(this.spritePosition.X + start + 2 * space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.GetWood().ToString(), new Vector2(this.spritePosition.X + start + 3 * space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.GetOre().ToString(), new Vector2(this.spritePosition.X + start + 4 * space, this.spritePosition.Y + 100), Color.White);

            //spriteBatch.DrawString(GameState.hudMaterialsFont, changeMaterials.ToString(), new Vector2(200, 200), Color.White);

            if (changeMaterials)
            {
                SourceAll sc = act.GetMaterialChange();


                if(sc.GetCorn() != 0) {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.GetCorn() > 0) ? "+" : "") + sc.GetCorn().ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 120), (sc.GetCorn() > 0) ? Color.Green : Color.Red);
                }

                if (sc.GetMeat() != 0)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.GetMeat() > 0) ? "+" : "") + sc.GetMeat().ToString(), new Vector2(this.spritePosition.X + start + space, this.spritePosition.Y + 120), (sc.GetMeat() > 0) ? Color.Green : Color.Red);
                }

                if (sc.GetOre() != 0)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.GetOre() > 0) ? "+" : "") + sc.GetOre().ToString(), new Vector2(this.spritePosition.X + start + 4 * space, this.spritePosition.Y + 120), (sc.GetOre() > 0) ? Color.Green : Color.Red);
                }

                if (sc.GetWood() != 0)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.GetWood() > 0) ? "+" : "") + sc.GetWood().ToString(), new Vector2(this.spritePosition.X + start + 3 * space, this.spritePosition.Y + 120), (sc.GetWood() > 0) ? Color.Green : Color.Red);
                }

                if (sc.GetStone() != 0)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.GetStone() > 0) ? "+" : "") + sc.GetStone().ToString(), new Vector2(this.spritePosition.X + start + 2 * space, this.spritePosition.Y + 120), (sc.GetStone() > 0) ? Color.Green : Color.Red);
                }
            }

            spriteBatch.End();
        }
    }
}
