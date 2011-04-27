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

        public void InitAIComponent(IMapController mapController, double[] koef)
        {
            this.mapController = mapController;
        }

        private void Recurse()
        {
            Recurse();
        }

        public void ResolveAI()
        {
            Recurse();

            int i = 0;
            //int a = 1 / i;

            TestGetDistance();

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
                myTown.BuildMonastery(1);
                mapController.InventUpgrade(SourceBuildingKind.Mine);
                mapController.InventUpgrade(SourceBuildingKind.Mine);
                mapController.InventUpgrade(SourceBuildingKind.Mine);
                mapController.InventUpgrade(SourceBuildingKind.Quarry);

                myTown.BuildMarket(0);

                mapController.BuyLicence(SourceKind.Corn);
                mapController.BuyLicence(SourceKind.Corn);
                mapController.BuyLicence(SourceKind.Corn);
                mapController.BuyLicence(SourceKind.Ore);
                mapController.BuyLicence(SourceKind.Ore);

                myTown.BuildMarket(2);
                mapController.BuyLicence(SourceKind.Ore);
                mapController.BuyLicence(SourceKind.Ore);

                for (int loop1 = -5; loop1 < 1000; loop1++)
                {
                    mapController.BuildRoad(loop1);
                }
            }
        }

        private void TestGetDistance()
        {
            ITown townID1 = mapController.GetITownByID(1);
            ITown townID2 = mapController.GetITownByID(2);
            ITown townID3 = mapController.GetITownByID(3);
            ITown townID47 = mapController.GetITownByID(47);
            mapController.GetDistance(townID3, townID47);

            if (mapController.GetDistance(townID1, townID1) != 0)
                throw new Exception("Get Distance error 1. 0 != " + mapController.GetDistance(townID1, townID1));
            if (mapController.GetDistance(townID1, townID2) != 3)
                throw new Exception("Get Distance error 2. 3 != " + mapController.GetDistance(townID1, townID2));
            if (mapController.GetDistance(townID1, townID3) != 1)
                throw new Exception("Get Distance error 3. 1 != " + mapController.GetDistance(townID1, townID3));
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIHacker();
            return component;
        }     
    }
}
