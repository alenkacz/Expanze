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

        RadioButtonComponent radio;

        String title = "";
        String selected = "";
        List<String> options = new List<String>();

        public MapSettingRowComponent(Game game, int x, int y, SpriteFont font, int width, int height, String title, List<String> options) 
            : base(game,x,y,font,width,height,null) 
        {
            this.title = title;
            this.options = options;
            this.selected = this.options.ElementAt(0);

            radio = new RadioButtonComponent(Settings.Game, x + 100, y, GameState.playerNameFont, 40, 40, "radiobutton_bg");

            spriteBatch = new SpriteBatch(myGame.GraphicsDevice);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            radio.Update(gameTime);
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

            spriteBatch.DrawString(GameState.playerNameFont, title, new Vector2(spritePosition.X, spritePosition.Y), Color.White);

            Vector2 position = new Vector2(spritePosition.X + 200,spritePosition.Y);

            foreach (String s in options)
            {
                // nessesary spacing
                position.X += 200;
                
                spriteBatch.DrawString(GameState.playerNameFont, s, position, Color.White);
            }

            radio.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
