using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum MapType { ISLAND, TWO_ISLANDS, LITTLE_ISLANDS }
    public enum MapSource { NORMAL, LOWLAND, WASTELAND }
    public enum MapProductivity { HIDDEN, HALF, VISIBLE }
    public enum MapKind { HIDDEN, HALF, VISIBLE }

    public interface IGameSetting
    {
        MapType GetMapType();
        MapSource GetMapSource();
        MapProductivity GetMapProductivity();
        MapKind GetMapKind();
    }
}
