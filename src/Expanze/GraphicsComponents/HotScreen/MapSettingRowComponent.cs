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
        //int start = 60;

        MouseState mouseState;
        int mousex;
        int mousey;
        bool pressed = false;

        // active player
        //private bool active = false;
        // if true, it means that this click was already catched - fix because add/rem buttons are on the same place
        //private bool alreadyChanged = false;

        RadioButtonComponent[] radio;

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

            radio = new RadioButtonComponent[3];

            radio[0] = new RadioButtonComponent(Settings.Game, (int)(spritePosition.X + 400), (int)spritePosition.Y + 10, GameResources.Inst().GetFont(EFont.PlayerNameFont), Settings.scaleW(27), Settings.scaleH(28));
            radio[1] = new RadioButtonComponent(Settings.Game, (int)(spritePosition.X + 650), (int)spritePosition.Y + 10, GameResources.Inst().GetFont(EFont.PlayerNameFont), Settings.scaleW(27), Settings.scaleH(28));
            radio[2] = new RadioButtonComponent(Settings.Game, (int)(spritePosition.X + 900), (int)spritePosition.Y + 10, GameResources.Inst().GetFont(EFont.PlayerNameFont), Settings.scaleW(27), Settings.scaleH(28));

            radio[1].SetSelected(true); //first one will be selected by default

            foreach (RadioButtonComponent r in radio)
                r.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (RadioButtonComponent r in radio)
                r.Update(gameTime);

            mouseState = Mouse.GetState();

            mousex = mouseState.X;
            mousey = mouseState.Y;

            if (ButtonState.Pressed == mouseState.LeftButton && !pressed)
            {
                pressed = true;

                for(int loop1 = 0; loop1 < 3; loop1++)
                {
                    if (radio[loop1].isInRange(mousex, mousey))
                    {
                        if (radio[loop1].GetIsActive())
                        {
                            for (int loop2 = 0; loop2 < 3; loop2++)
                                radio[loop2].SetSelected(false);

                            radio[loop1].SetSelected(true);
                        }
     
                        break;
                    }
                }
            }

            if (pressed && ButtonState.Pressed != mouseState.LeftButton)
            {
                pressed = false;
            }
        }

        public String getSelectedSettings()
        {
            for (int loop1 = 0; loop1 < 3; loop1++)
            {
                if (radio[loop1].isSelected())
                    return options[loop1];
            }

            return "";
        }

        public void SetActiveRadio(bool active, int which)
        {
            radio[which].SetIsActive(active);
            if (!active && radio[which].isSelected())
            {
                radio[which].SetSelected(false);
                radio[(which + 1) % 3].SetSelected(true);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Color c;
            c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,Settings.spriteScale);

            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.PlayerNameFont), title, new Vector2(spritePosition.X, spritePosition.Y), Color.White);

            Vector2 position = new Vector2(spritePosition.X + 200,spritePosition.Y);

            foreach (RadioButtonComponent r in radio)
                r.Draw(gameTime);

            foreach (String s in options)
            {
                // nessesary spacing
                position.X += 250;

                spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.PlayerNameFont), s, position, Color.White);
            }

            spriteBatch.End();
        }
    }
}
