using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class MapSettingRowComponent : GuiComponent
    {

        //space between texts in HUD of materials
        const int space = 150;
        int start = 60;

        // active player
        private bool active = false;
        // if true, it means that this click was already catched - fix because add/rem buttons are on the same place
        private bool alreadyChanged = false;

        RadioButtonComponent radio1;
        RadioButtonComponent radio2;
        RadioButtonComponent radio3;

        String title = "";
        String selected = "";
        List<String> options = new List<String>();

        public MapSettingRowComponent(Game game, int x, int y, SpriteFont font, int width, int height, String title, List<String> options) 
            : base(game,x,y,font,width,height,null) 
        {
            this.title = title;
            this.options = options;
            this.selected = this.options.ElementAt(0);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            radio1 = new RadioButtonComponent(Settings.Game, (int)(spritePosition.X + 350), (int)spritePosition.Y + 10, GameState.playerNameFont, 40, 40);
            radio2 = new RadioButtonComponent(Settings.Game, (int)(spritePosition.X + 550), (int)spritePosition.Y + 10, GameState.playerNameFont, 40, 40);
            radio3 = new RadioButtonComponent(Settings.Game, (int)(spritePosition.X + 750), (int)spritePosition.Y + 10, GameState.playerNameFont, 40, 40);
            radio1.LoadContent();
            radio2.LoadContent();
            radio3.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            radio1.Update(gameTime);
            radio2.Update(gameTime);
            radio3.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Color c;
            c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);

            spriteBatch.DrawString(GameState.playerNameFont, title, new Vector2(spritePosition.X, spritePosition.Y), Color.White);

            Vector2 position = new Vector2(spritePosition.X + 200,spritePosition.Y);

            radio1.Draw(gameTime);
            radio2.Draw(gameTime);
            radio3.Draw(gameTime);

            foreach (String s in options)
            {
                // nessesary spacing
                position.X += 200;
                
                spriteBatch.DrawString(GameState.playerNameFont, s, position, Color.White);
            }

            spriteBatch.End();
        }
    }
}
