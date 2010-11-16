﻿using System;
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
        IMapController mapController;

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;
        }

        public void ResolveAI()
        {
            if (mapController.GetState() == EGameState.StateFirstTown)
            {
                int id = mapController.GetHexa(0, 0).getITown(TownPos.Up).getTownID();
                if (mapController.BuildTown(id) != TownBuildError.OK)
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
