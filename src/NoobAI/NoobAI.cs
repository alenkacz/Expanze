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
                for (int loop1 = 13; loop1 < 50; loop1 += 5)
                {
                    if (mapController.BuildTown(loop1) == TownBuildError.OK)
                    {
                        break;
                    }
                }
            }
            else if (mapController.GetState() == EGameState.StateSecondTown)
            {
                for(int loop1 = 20; loop1 < 50; loop1 += 5)
                {
                    if (mapController.BuildTown(loop1) == TownBuildError.OK)
                    {
                        break;
                    }
                }
                while (true)
                {
                }
            }
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new NoobAI();
            return component;
        }
    }
}
