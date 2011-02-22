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

        ITown myTown;

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
            int i = 0;
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
                ITown tempTown;
                for (int loop1 = -5; loop1 < 30; loop1++)
                {
                    tempTown = mapController.BuildTown(loop1);
                    if (tempTown != null)
                    {
                        myTown = tempTown;
                        break;
                    }
                }
            }
            else
            {
                IMonastery mon1 = myTown.BuildMonastery(1);
                mon1.InventUpgrade(SourceBuildingKind.Mill);
                mon1.InventUpgrade(SourceBuildingKind.Stepherd);
                mon1.InventUpgrade(SourceBuildingKind.Stepherd);

                IMarket market1 = myTown.BuildMarket(0);
                
                market1.BuyLicence(SourceKind.Corn);
                market1.BuyLicence(SourceKind.Wood);
                market1.BuyLicence(SourceKind.Meat);
                market1.BuyLicence(SourceKind.Ore);

                IMarket market2 = myTown.BuildMarket(2);
                market2.BuyLicence(SourceKind.Ore);
                market2.BuyLicence(SourceKind.Stone);
                market2.BuyLicence(SourceKind.Stone);

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
