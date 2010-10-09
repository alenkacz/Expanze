using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

public enum KEY_STATE
{
    NEW_PRESS = 1,
    PRESS = 2,
    NEW_RELEASE = 3,
    RELEASE = 4
}

namespace tutorial
{
    /// <summary>
    /// Hlavn� t��da GameClass d�d�c� od t��dy Game, kter� je z�kladn� stavebn� k�men XNA aplikace.
    /// </summary>
    public class GameClass : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        // code 1
        Texture2D myTexture;
        Vector2 spritePosition;
        String myText;
        KeyboardState currKeyboardState;
        KeyboardState lastKeyboardState;

        // code 2
        CustomCursor cursorComp;

        // code 3
        Draw3D draw3dComp;

        public GameClass()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            base.IsMouseVisible = false;

            this.Window.Title = "Cviceni 3 Base";
            Content.RootDirectory = "Content";
            cursorComp = new CustomCursor(this);
            Components.Add(cursorComp);
            draw3dComp = new Draw3D(this);
            Components.Add(draw3dComp);
            myText = "OFF";
        }

        /// <summary>
        /// Dovoluje inicializovat v�ci pot�ebn� p�ed samotn�m spu�t�n�m.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent je zavol�na jedou na za��tku
        /// V�echen n� obsah nahrajeme pr�v� v t�to metod�
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("myFont");
            myTexture = Content.Load<Texture2D>("menuItem");

            spritePosition = new Vector2(graphics.PreferredBackBufferWidth / 2 - myTexture.Width / 2,
                   graphics.PreferredBackBufferHeight / 8 - myTexture.Height / 2);

        }

        /// <summary>
        /// Funkce se vol� jednou za b�hu aplikace a m� na starost odalokov�n�
        /// na�ten�ch hern�m dat
        /// </summary>
        protected override void UnloadContent()
        {

        }

        private bool IsKeyState(Keys key, KEY_STATE state)
        {
            switch (state)
            {
                case KEY_STATE.NEW_PRESS :
                    return (currKeyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key));
                case KEY_STATE.NEW_RELEASE :
                    return (currKeyboardState.IsKeyUp(key) && !lastKeyboardState.IsKeyUp(key));
                case KEY_STATE.PRESS:
                    return (currKeyboardState.IsKeyDown(key));
                case KEY_STATE.RELEASE:
                    return (currKeyboardState.IsKeyUp(key));
            }

            return false;
        }

        /// <summary>
        /// Umo�uje d�ky neust�l�mu vol�n� aktualizovat hern� logiku, sv�t
        /// nebo t�eba detekci koliz�
        /// </summary>
        /// <param name="gameTime">�asov� instance (XNA si po��t� samo)</param>
        protected override void Update(GameTime gameTime)
        {
            lastKeyboardState = currKeyboardState;
            currKeyboardState = Keyboard.GetState();
      
            // Code 1
            // pohyb spritem podle navracene hodnoty z IsKeyState(...
            double change = 50 * gameTime.ElapsedGameTime.Milliseconds / 1000.0;
            if (IsKeyState(Keys.Up, KEY_STATE.PRESS))
                spritePosition.Y -= (float) change;
            if (IsKeyState(Keys.Down, KEY_STATE.PRESS))
                spritePosition.Y += (float) change;
            if (IsKeyState(Keys.Left, KEY_STATE.PRESS))
                spritePosition.X -= (float) change;
            if (IsKeyState(Keys.Right, KEY_STATE.PRESS))
                spritePosition.X += (float) change;
            // Code 1
            // zmena textu, naimplementovat NEW_PRESS v IsKeyState
            //if (IsKeyState(Keys.Enter, KEY_STATE.PRESS))
            if (IsKeyState(Keys.Enter, KEY_STATE.NEW_PRESS))
            {
                if (myText == "OFF")
                    myText = "ON";
                else
                    myText = "OFF";
            }

            // Pokud budete mit hotovy Code 3 a zaroven ulozenou draw3d komponentu v draw3dComp, tak muzete ovladat pohyb modelu i odtud :)
            //draw3dComp.world *= Matrix.CreateRotationY(MathHelper.ToRadians(5));

            base.Update(gameTime);
        }

        /// <summary>
        /// Vykreslovac� metoda. Stejn� jako Update() se neust�l� vol�,
        /// proto�e je sou��st� hern� smy�ky
        /// </summary>
        /// <param name="gameTime">�asov� instance (XNA si po��t� samo)</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(myTexture, spritePosition, Color.White);
            spriteBatch.DrawString(spriteFont, myText, new Vector2(spritePosition.X + 10, spritePosition.Y + 5), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
