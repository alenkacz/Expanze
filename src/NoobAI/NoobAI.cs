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
                mapController.BuildTown(FindBestFreeTownID());
                /*
                int id = mapController.GetHexa(0, 0).getITown(TownPos.Up).getTownID();
                for (int loop1 = 13; loop1 < 50; loop1 += 5)
                {
                    if (mapController.BuildTown(loop1) == TownBuildError.OK)
                    {
                        break;
                    }
                }*/
            }
            else if (mapController.GetState() == EGameState.StateSecondTown)
            {
                mapController.BuildTown(FindBestFreeTownID());
                /*
                for(int loop1 = 20; loop1 < 50; loop1 += 5)
                {
                    if (mapController.BuildTown(loop1) == TownBuildError.OK)
                    {
                        break;
                    }
                }*/
            }
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new NoobAI();
            return component;
        }


        public int FindBestFreeTownID()
        {
            int maxTownID = mapController.GetMaxTownID();
            int maxSum = 0;
            int maxSumTownID = -1;

            for (int loop1 = 1; loop1 < maxTownID; loop1++)
            {
                int sourceSum = 0;
                ITownGet town = mapController.GetITownGetByID(loop1);
                if (town.CanActivePlayerBuildTown() != TownBuildError.OK)
                    continue;
                for (int loop2 = 0; loop2 < 3; loop2++)
                {
                    IHexaGet hexa = town.getIHexaGet(loop2);
                    if (hexa != null)
                    {
                        sourceSum += hexa.getCurrentSource();
                    }
                }
                if (sourceSum > maxSum)
                {
                    maxSum = sourceSum;
                    maxSumTownID = loop1;
                }
            }
            return maxSumTownID;
        }
    }
}
