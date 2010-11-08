using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.MapGeneration
{
    interface IMapController
    {
        IHexaGet GetHexa(int x, int y);
        bool BuildTown(int townID);
    }
}
