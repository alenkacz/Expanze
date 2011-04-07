using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum HexaKind { Cornfield, Pasture, Stone, Forest, Mountains, Desert, Water, Nothing, Null };
    // use for positioning roads and hexas too
    public enum RoadPos { UpLeft, UpRight, MiddleLeft, MiddleRight, BottomLeft, BottomRight, Count };
    public enum TownPos { Up, UpRight, BottomRight, Bottom, BottomLeft, UpLeft, Count };

    public interface IHexa
    {
        HexaKind GetKind();
        SourceKind GetSourceKind();
        int GetStartSource();
        int GetCurrentSource();
        ITown GetITown(TownPos townPos);
        int GetID();
        IHexa GetIHexaNeighbour(RoadPos pos);
        bool GetCaptured();
        IPlayer GetCapturedIPlayer();
        ISourceAll GetSourceBuildingCost();

        /// <summary>
        /// How many sources normaly get player from that hexa.
        /// It is affected by Monastery upgrades
        /// </summary>
        /// <param name="player"></param>
        /// <returns>Sum of source building productivities</returns>
        int GetNormalProductivity(IPlayer player);
    }
}
