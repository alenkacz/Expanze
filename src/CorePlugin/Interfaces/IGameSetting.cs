using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum MapSize { SMALL, MEDIUM, BIG }
    public enum MapType { NORMAL, LOWLAND, WASTELAND }
    public enum MapWealth { LOW, MEDIUM, HIGH }

    public interface IGameSetting
    {
        MapSize GetMapSize();
        MapType GetMapType();
        MapWealth GetMapWealth();
        int GetWinningPoints();
    }
}
