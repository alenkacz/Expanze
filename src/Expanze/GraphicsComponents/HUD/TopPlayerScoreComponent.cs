using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze
{
    class TopPlayerScoreComponent : GuiComponent
    {
        Texture2D textureLeft;
        Texture2D textureRight;
        Texture2D textureMiddle;
        SpriteBatch spriteBatch;

        Vector2 positionLeft;
        Vector2 positionRight;
        Vector2 positionMiddle;
        Vector2 positionName;

        protected Boolean pick;

        public TopPlayerScoreComponent() 
        {
            positionLeft = new Vector2((int)Settings.maximumResolution.X - 300, 0);
            positionRight = new Vector2((int)Settings.maximumResolution.X - 79, 0);
            positionMiddle = new Vector2((int)Settings.maximumResolution.X - 300 + 11, 0);
            positionName = new Vector2((int)Settings.maximumResolution.X - 250, 0);
        }

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Settings.Game.GraphicsDevice);
            textureLeft = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-left");
            textureRight = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-right");
            textureMiddle = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-middle");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            Color c;
            c = Color.White;

            spriteBatch.Draw(textureLeft, positionLeft, c);
            spriteBatch.Draw(textureRight, positionRight, c);
            spriteBatch.DrawString(GameState.gameFont, "jmeno", positionName, Color.White);

            spriteBatch.End();
        }

    }
}
