using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expanze.MapGeneration;

namespace Expanze.AI
{
    interface IComponentAI
    {
        void ResolveAI(IMapController mapController);
    }
}
