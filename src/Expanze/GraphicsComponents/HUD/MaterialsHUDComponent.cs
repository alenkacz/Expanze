﻿using System;
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

            Player act = GameMaster.Inst().GetTargetPlayer();

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
                c = Color.Black;
            else
                c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);
            spritePosition.Y = 755;
            spritePosition.X = 350;

            Player act = GameMaster.Inst().GetTargetPlayer();

            spriteBatch.Draw(myButton, spritePosition, c);
            if (pick)
            {
                spriteBatch.End();
                return;
            }

            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.getCorn().ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.getMeat().ToString(), new Vector2(this.spritePosition.X + start + space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.getStone().ToString(), new Vector2(this.spritePosition.X + start + 2 * space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.getWood().ToString(), new Vector2(this.spritePosition.X + start + 3 * space, this.spritePosition.Y + 100), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), act.getOre().ToString(), new Vector2(this.spritePosition.X + start + 4 * space, this.spritePosition.Y + 100), Color.White);

            //spriteBatch.DrawString(GameState.hudMaterialsFont, changeMaterials.ToString(), new Vector2(200, 200), Color.White);

            if (changeMaterials)
            {
                SourceAll sc = act.GetMaterialChange();


                if(sc.corn != 0) {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.corn > 0) ? "+" : "") + sc.corn.ToString(), new Vector2(this.spritePosition.X + start, this.spritePosition.Y + 120), (sc.corn > 0) ? Color.Green : Color.Red);
                }

                if (sc.meat != 0)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.meat > 0) ? "+" : "") + sc.meat.ToString(), new Vector2(this.spritePosition.X + start + space, this.spritePosition.Y + 120), (sc.meat > 0) ? Color.Green : Color.Red);
                }

                if (sc.ore != 0)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.ore > 0) ? "+" : "") + sc.ore.ToString(), new Vector2(this.spritePosition.X + start + 4 * space, this.spritePosition.Y + 120), (sc.ore > 0) ? Color.Green : Color.Red);
                }

                if (sc.wood != 0)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.wood > 0) ? "+" : "") + sc.wood.ToString(), new Vector2(this.spritePosition.X + start + 3 * space, this.spritePosition.Y + 120), (sc.wood > 0) ? Color.Green : Color.Red);
                }

                if (sc.stone != 0)
                {
                    spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((sc.stone > 0) ? "+" : "") + sc.stone.ToString(), new Vector2(this.spritePosition.X + start + 2 * space, this.spritePosition.Y + 120), (sc.stone > 0) ? Color.Green : Color.Red);
                }
            }

            spriteBatch.End();
        }
    }
}
