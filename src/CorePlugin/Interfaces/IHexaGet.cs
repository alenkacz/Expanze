using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum HexaKind { Cornfield, Forest, Stone, Pasture, Mountains, Desert, Water, Nothing, Null };
    // use for positioning roads and hexas too
    public enum RoadPos { UpLeft, UpRight, MiddleLeft, MiddleRight, BottomLeft, BottomRight, Count };
    public enum TownPos { Up, UpRight, BottomRight, Bottom, BottomLeft, UpLeft, Count };

    public interface IHexaGet
    {
        HexaKind getKind();
        int getStartSource();
        int getCurrentSource();
        ITownGet getITown(TownPos townPos);
        int getID();
    }
}
