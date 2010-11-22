using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze
{
    static class Settings
    {
        public static Vector2[] allResolutions = { new Vector2(800, 600), new Vector2(1024, 768), new Vector2(1280, 800), new Vector2(1366, 768) };
        public static Vector2 maximumResolution = new Vector2(1440,900);

        //indexed by enum Types
        public static String[] mapPaths = new String[] { "Models/yellowhex", "Models/brownhex", "Models/greyhex", "Models/greenhex", "Models/redhex", "Models/orangehex", "Models/bluehex" };

        //// resources which get player on start
        //public static SourceAll startResources = new SourceAll(150);

        // costs of infrastructure
        public static SourceAll costTown = new SourceAll(60, 60, 50, 50, 30);
        public static SourceAll costRoad = new SourceAll(0, 40, 0, 0, 50);
        public static SourceAll costMine = new SourceAll(0, 50, 0, 50, 0);
        public static SourceAll costSaw = new SourceAll(0, 0, 0, 50, 50);
        public static SourceAll costMill = new SourceAll(50, 50, 0, 0, 0);
        public static SourceAll costStephard = new SourceAll(0, 0, 50, 0, 50);
        public static SourceAll costQuarry = new SourceAll(50, 0, 50, 0, 0);

        // goals
        public const int pointsWin = 40; /// how much points have to have player to win
        public const int pointsTown = 5; /// points for new town
        public const int pointsRoad = 1; /// points for new road


        public static Vector2 activeResolution = new Vector2(800,600);

        public static float getScale()
        {
            return activeResolution.X / maximumResolution.X;
        }

        public static GraphicsDeviceManager GraphicsDeviceManager = null;
        public static Game Game = null;

        public static bool isFullscreen = false;
        public static int conversionRate = 4;

        public static Matrix spriteScale = Matrix.CreateScale(Settings.activeResolution.X / Settings.maximumResolution.X, Settings.activeResolution.Y / Settings.maximumResolution.Y, 1);

        public static void scaleChange()
        {
            spriteScale = Matrix.CreateScale(Settings.activeResolution.X / Settings.maximumResolution.X, Settings.activeResolution.Y / Settings.maximumResolution.Y, 1);
        }

        public static Vector2 scale(Vector2 size)
        {
            return new Vector2(size.X * spriteScale.M11,size.Y * spriteScale.M22);
        }

        public static int scaleW(float w)
        {
            return (int)(w * spriteScale.M11);
        }

        public static int scaleH(float h)
        {
            return (int)(h * spriteScale.M22);
        }

        //position of player name on the gamescreen
        public static Vector2 playerNamePosition = new Vector2(Settings.maximumResolution.X - scaleW(500), scaleH(15));
        public static Vector2 playerColorPosition = new Vector2(Settings.maximumResolution.X - scaleW(200), scaleH(28));
        public static Vector2 playerColorSize = new Vector2(scaleW(50), scaleH(50));
    }
}
