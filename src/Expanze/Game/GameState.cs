﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Gameplay.Map;

namespace Expanze
{
    static class GameState
    {
        public static Matrix view;
        public static Matrix projection;
        public static Vector3 MaterialAmbientColor;
        public static Vector3 LightDirection;
        public static Vector3 LightDiffusionColor;
        public static Vector3 LightSpecularColor;

        public static MouseState CurrentMouseState;
        public static MouseState LastMouseState;
        public static KeyboardState CurrentKeyboardState;

        public static Game game;
        public static Map map;

        public static Message message;

        public static SpriteBatch spriteBatch;

        public static SpriteFont gameFont;
        public static SpriteFont playerNameFont;
        public static SpriteFont hudMaterialsFont;
        public static SpriteFont materialsNewFont;
        public static SpriteFont medievalSmall;
        public static SpriteFont medievalMedium;
        public static SpriteFont medievalBig;
    }
}
