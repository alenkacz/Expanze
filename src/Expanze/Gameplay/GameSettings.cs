using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze
{
    class GameSettings : IGameSetting
    {

        int points = 0;
        MapType mapType;
        MapSize mapSize;
        MapWealth mapWealth;

        public GameSettings(int points, string mapType, string mapSize, string mapWealth) 
        {
            this.points = points;

            if (mapType == Strings.GAME_SETTINGS_MAP_TYPE_LOWLAND)
                this.mapType = MapType.LOWLAND;
            else if (mapType == Strings.GAME_SETTINGS_MAP_TYPE_NORMAL)
                this.mapType = MapType.NORMAL;
            else
                this.mapType = MapType.WASTELAND;

            if (mapSize == Strings.GAME_SETTINGS_MAP_SIZE_SMALL)
                this.mapSize = MapSize.SMALL;
            else if (mapSize == Strings.GAME_SETTINGS_MAP_SIZE_MEDIUM)
                this.mapSize = MapSize.MEDIUM;
            else
                this.mapSize = MapSize.BIG;

            if (mapWealth == Strings.GAME_SETTINGS_MAP_WEALTH_LOW)
                this.mapWealth = MapWealth.LOW;
            else if (mapWealth == Strings.GAME_SETTINGS_MAP_WEALTH_MEDIUM)
                this.mapWealth = MapWealth.MEDIUM;
            else
                this.mapWealth = MapWealth.HIGH;
        }

        public int GetPoints()
        {
            return points;
        }

        public string GetMapSizeXML() 
        {
            switch (mapSize)
            {
                case MapSize.SMALL: return "small";
                case MapSize.MEDIUM: return "medium";
                case MapSize.BIG: return "big";
            }
            return "";
        }

        public string GetMapTypeXML()
        {
            switch (mapType)
            {
                case MapType.NORMAL: return "normal";
                case MapType.LOWLAND: return "lowland";
                case MapType.WASTELAND: return "wasteland";
            }
            return "";
        }

        public string GetMapWealthXML()
        {
            switch (mapWealth)
            {
                case MapWealth.LOW: return "low";
                case MapWealth.MEDIUM: return "medium";
                case MapWealth.HIGH: return "high";
            }
            return "";
        }


        #region IGameSetting Members

        public MapSize GetMapSize()
        {
            return mapSize;
        }

        public MapType GetMapType()
        {
            return mapType;
        }

        public MapWealth GetMapWealth()
        {
            return mapWealth;
        }

        public int GetWinningPoints()
        {
            return points;
        }

        #endregion
    }
}
