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
        MapType mapType;
        MapSource mapSource;
        MapProductivity mapProductivity;
        MapKind mapKind;
        int playerCount;

        public int PlayerCount
        {
            get { return playerCount; }
            set { playerCount = value; }
        }

        public GameSettings(int playerCount, string mapTypeS, string mapSourceS, string mapKindS, string mapProductivityS) 
        {
            this.playerCount = playerCount;

            if (mapTypeS == Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_TYPE_ISLAND))
                mapType = MapType.ISLAND;
            else if (mapTypeS == Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_TYPE_2_ISLANDS))
                mapType = MapType.TWO_ISLANDS;
            else
                mapType = MapType.LITTLE_ISLANDS;

            if (mapSourceS == Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_SOURCE_LOWLAND))
                mapSource = MapSource.LOWLAND;
            else if (mapSourceS == Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_SOURCE_NORMAL))
                mapSource = MapSource.NORMAL;
            else
                mapSource = MapSource.WASTELAND;

            if (mapKindS == Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_KIND_HIDDEN))
                mapKind = MapKind.HIDDEN;
            else if (mapKindS == Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_KIND_HALF))
                mapKind = MapKind.HALF;
            else
                mapKind = MapKind.VISIBLE;

            if (mapProductivityS == Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_PRODUCTIVITY_HIDDEN))
                mapProductivity = MapProductivity.HIDDEN;
            else if (mapProductivityS == Strings.Inst().GetString(TextEnum.GAME_SETTINGS_MAP_PRODUCTIVITY_HALF))
                mapProductivity = MapProductivity.HALF;
            else
                mapProductivity = MapProductivity.VISIBLE;
        }


        #region IGameSetting Members

        public MapType GetMapType()
        {
            return mapType;
        }

        public MapSource GetMapSource()
        {
            return mapSource;
        }

        public MapProductivity GetMapProductivity()
        {
            return mapProductivity;
        }

        public MapKind GetMapKind()
        {
            return mapKind;
        }

        #endregion
    }
}
