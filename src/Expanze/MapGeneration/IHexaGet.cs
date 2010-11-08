using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.MapGeneration
{
    public enum HexaType { Cornfield, Forest, Stone, Pasture, Mountains, Desert, Water, Nothing, Null };
    // use for positioning roads and hexas too
    public enum RoadPos { UpLeft, UpRight, MiddleLeft, MiddleRight, BottomLeft, BottomRight, Count };
    public enum TownPos { Up, UpLeft, UpRight, BottomLeft, BottomRight, Bottom, Count };

    interface IHexaGet
    {
        HexaType getType();
        ITownGet getITown(TownPos townPos);
    }
}
