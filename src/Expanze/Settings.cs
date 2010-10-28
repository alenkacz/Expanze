using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Expanze.Gameplay;

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
        public static Vector2 playerNamePosition = new Vector2(790,10);

        public static SourceCost costTown = new SourceCost(60, 60, 50, 50, 30);
        public static SourceCost costRoad = new SourceCost(0, 40, 0, 0, 50);
    }
}
