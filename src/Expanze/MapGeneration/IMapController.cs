using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.MapGeneration
{
    public enum EGameState { StateFirstTown, StateSecondTown, StateGame };

    interface IMapController
    {
        IHexaGet GetHexa(int x, int y);
        bool BuildTown(int townID);
        EGameState GetState();
    }
}
