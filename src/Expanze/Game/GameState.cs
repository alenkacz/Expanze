using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Gameplay.Map;

namespace Expanze
{
    public enum PickingState
    {
        onlyNormal,
        normalAndPicking,
        onlyPicking
    }

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

        public static SpriteBatch spriteBatch;

        public static HotSeatScreen hotSeatScreen;

        public static bool wireModel = false;
        public static RasterizerState rasterizerState = new RasterizerState();
        public static PickingState pickingTexture = PickingState.onlyNormal;
        public static bool debugInfo = false;
    }
}
