using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.MapGeneration
{
    public enum HexaType { Cornfield, Forest, Stone, Pasture, Mountains, Desert, Water, Nothing, Null };

    interface IHexaGet
    {
        HexaType getType();
    }
}
