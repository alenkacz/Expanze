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
        ITown town;


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
                town = mapController.BuildTown(5);
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
                town.CanBuildTown();
                ISourceBuilding source = town.BuildSourceBuilding(0);
                
                town.BuildSourceBuilding(1);
                town.BuildSourceBuilding(2);
            }
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIEasy();
            return component;
        }     
    }
}
