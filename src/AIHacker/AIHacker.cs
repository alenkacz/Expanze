using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using CorePlugin.Attributes;

namespace AIHacker
{
    [PluginAttributeAI("Hacker, not real ai, tries to cheat")]
    class AIHacker : IComponentAI
    {
        IMapController mapController;

        public String GetAIName()
        {
            return "AI - Hacker";
        }

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;
        }

        public void ResolveAI()
        {
            //int i = 0;
            //int a = 1 / i;
            if (mapController.GetState() == EGameState.StateFirstTown)
            {
                for (int loop1 = -5; loop1 < 20; loop1++)
                {
                    mapController.BuildTown(loop1);
                }
            }
            else if (mapController.GetState() == EGameState.StateSecondTown)
            {
                for (int loop1 = -5; loop1 < 30; loop1++)
                {
                    mapController.BuildTown(loop1);
                }
            }
            else
            {
                for (int loop1 = -5; loop1 < 1000; loop1++)
                {
                    mapController.BuildRoad(loop1);
                }
            }
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIHacker();
            return component;
        }     
    }
}
