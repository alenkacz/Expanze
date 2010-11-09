using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public interface IComponentAI
    {
        void ResolveAI(IMapController mapController);
    }
}
