using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expanze.MapGeneration;

namespace Expanze.AI
{
    class NoobAI : IComponentAI
    {
        public void ResolveAI(IMapController mapController)
        {
            HexaType type = mapController.GetHexa(0,0).getType();
            
        }
    }
}
