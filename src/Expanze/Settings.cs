using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze
{
    static class Settings
    {
        public enum Types { Cornfield, Forest, Stone, Pasture, Mountains, Desert, Water, Nothing, Null };
        public static Vector2[] allResolutions = { new Vector2(800, 600), new Vector2(1024, 768), new Vector2(1366, 768) };
        public static Vector2 maximumResolution = new Vector2(1440,900);

        //indexed by enum Types
        public static String[] mapPaths = new String[] { "Models/yellowhex", "Models/brownhex", "Models/greyhex", "Models/greenhex", "Models/redhex", "Models/orangehex", "Models/bluehex" };
        
        //score at the beginning
        public const int startScore = 15000;

        public static SourceCost costTown = new SourceCost(60, 60, 50, 50, 30);
        public static SourceCost costRoad = new SourceCost(0, 40, 0, 0, 50);

        public static Vector2 activeResolution = new Vector2(800,600);

        //position of player name on the gamescreen
        public static Vector2 playerNamePosition = new Vector2(Settings.activeResolution.X-80, 10);

        public static float getScale()
        {
            return activeResolution.X / maximumResolution.X;
        }

        public static GraphicsDeviceManager GraphicsDeviceManager = null;

        public static bool isFullscreen = false;

        public static Matrix spriteScale = Matrix.CreateScale(Settings.activeResolution.X / Settings.maximumResolution.X, Settings.activeResolution.Y / Settings.maximumResolution.Y, 1);

        public static void scaleChange()
        {
            spriteScale = Matrix.CreateScale(Settings.activeResolution.X / Settings.maximumResolution.X, Settings.activeResolution.Y / Settings.maximumResolution.Y, 1);
            playerNamePosition = new Vector2(Settings.activeResolution.X - 70, 5);
        }

        public static int scale(float v)
        {
            return (int)( spriteScale.M11 * v);
        }
    }
}
