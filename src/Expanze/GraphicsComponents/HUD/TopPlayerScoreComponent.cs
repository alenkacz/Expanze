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
    class TopPlayerScoreComponent : GameComponent
    {
        Texture2D textureLeft;
        Texture2D textureRight;
        Texture2D textureMiddle;
        SpriteBatch spriteBatch;

        Vector2 position;

        protected Boolean pick;

        public TopPlayerScoreComponent() { }

        public TopPlayerScoreComponent(Game game, int x, int y, SpriteFont font, int width, int height, String texture)
        {
            position = new Vector2((int)Settings.maximumResolution.X - 300, 0);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(Settings.Game.GraphicsDevice);
            textureLeft = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-left");
            textureRight = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-right");
            textureMiddle = Settings.Game.Content.Load<Texture2D>("HUD/hud-top-middle");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, Boolean pick)
        {
            this.pick = pick;
            Draw(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
            Color c;
            c = Color.White;

            spriteBatch.Draw(textureLeft, position, c);
            spriteBatch.DrawString(GameState.gameFont, "jmeno", new Vector2(position.X+100,position.Y), Color.White);

            spriteBatch.End();
        }

    }
}
