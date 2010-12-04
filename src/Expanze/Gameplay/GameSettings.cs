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
            this.mapType = mapType.ToLower();
            this.mapSize = mapSize.ToLower();
            this.mapWealth = mapWealth.ToLower();
        }

        public string getMapSize() 
        {
            if (mapSize == "malá")
            {
                return "small";
            }
            else if (mapSize == "střední")
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
            if (mapType == "normální")
            {
                return "normal";
            }
            else if (mapType == "nížiny")
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
            if (mapWealth == "nízké")
            {
                return "low";
            }
            else if (mapWealth == "střední")
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
