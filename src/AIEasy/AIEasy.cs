using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using CorePlugin.Attributes;

namespace AIEasy
{
    [PluginAttributeAI("Easy AI")]
    class AIEasy : IComponentAI
    {
        IMapController mapController;

        public String GetAIName()
        {
            return "AI - Easy";
        }

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;
        }

        public void ResolveAI()
        {
            if (mapController.GetState() == EGameState.StateFirstTown)
            {
                mapController.BuildTown(mapController.GetHexa(1, 2).getITown(TownPos.BottomRight));
                mapController.BuildTown(5);
                mapController.BuildTown(10);
                mapController.BuildTown(15);
            }
            else if (mapController.GetState() == EGameState.StateSecondTown)
            {
                mapController.BuildTown(20);
                mapController.BuildTown(25);
                mapController.BuildTown(30);
            }
            else
            {
                
            }
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIEasy();
            return component;
        }     
    }
}
