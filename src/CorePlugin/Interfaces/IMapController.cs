using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum EGameState { StateFirstTown, StateSecondTown, StateGame };

    public interface IMapController
    {
        IHexaGet GetHexa(int x, int y);
        TownBuildError BuildTown(int townID);
        RoadBuildError BuildRoad(int roadID);
        EGameState GetState();
    }
}
