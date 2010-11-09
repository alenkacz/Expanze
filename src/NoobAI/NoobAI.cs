using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using CorePlugin.Attributes;

namespace NoobAI
{
    [PluginAttributeAI("Really dumb AI")]
    class NoobAI : IComponentAI
    {
        public void ResolveAI(IMapController mapController)
        {
            if (mapController.GetState() == EGameState.StateFirstTown)
            {
                int id = mapController.GetHexa(0, 0).getITown(TownPos.Up).getTownID();
                if (mapController.BuildTown(id) == false)
                {
                    mapController.BuildTown(12);
                }
            }
            else if (mapController.GetState() == EGameState.StateSecondTown)
            {
                mapController.BuildTown(31);
            }
        }
    }
}
