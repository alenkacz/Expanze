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

        public int getPoints()
        {
            return points;
        }

        public string getMapSize() 
        {
            if (mapSize == Strings.GAME_SETTINGS_MAP_SIZE_SMALL)
            {
                return "small";
            }
            else if (mapSize == Strings.GAME_SETTINGS_MAP_SIZE_MEDIUM)
            {
                return "medium";
            }
            else
            {
                return "big";
            }
        }

        public string getMapType()
        {
            if (mapType == Strings.GAME_SETTINGS_MAP_TYPE_NORMAL)
            {
                return "normal";
            }
            else if (mapType == Strings.GAME_SETTINGS_MAP_TYPE_LOWLAND)
            {
                return "lowland";
            }
            else
            {
                return "wasteland";
            }
        }

        public string getMapWealth()
        {
            if (mapWealth == Strings.GAME_SETTINGS_MAP_WEALTH_LOW)
            {
                return "low";
            }
            else if (mapWealth == Strings.GAME_SETTINGS_MAP_WEALTH_MEDIUM)
            {
                return "medium";
            }
            else
            {
                return "high";
            }
        }

    }
}
