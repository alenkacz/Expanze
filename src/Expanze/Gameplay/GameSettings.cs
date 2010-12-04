using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze
{
    class GameSettings
    {

        int points = 0;
        string mapType = "";
        string mapSize = "";
        string mapWealth = "";

        public GameSettings(int points, string mapType, string mapSize, string mapWealth) 
        {
            this.points = points;
            this.mapType = mapType;
            this.mapSize = mapSize;
            this.mapWealth = mapWealth;
        }

    }
}
