using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze
{
    static class GameState
    {
        public static Matrix view;
        public static Matrix projection;

        public static MouseState CurrentMouseState;
        public static MouseState LastMouseState;

        public static Game game;
        public static Map map;

        public static SpriteBatch spriteBatch;
        public static SpriteFont gameFont;
        public static SpriteFont hudMaterialsFont;
        public static SpriteFont materialsNewFont;
    }
}
