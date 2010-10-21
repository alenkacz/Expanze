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

        //indexed by enum Types
        public static String[] mapPaths = new String[] { "Models/yellowhex", "Models/brownhex", "Models/greyhex", "Models/greenhex", "Models/redhex", "Models/orangehex", "Models/bluehex" };
        
        //score at the beginning
        public const int startScore = 15000;

        //position of player name on the gamescreen
        public static Vector2 playerNamePosition = new Vector2(10,10);

        public static Matrix view;
        public static Matrix projection;
    }
}
