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

        int woodHexa;
        int cornHexa;
        int meatHexa;
        int oreHexa;
        int stoneHexa;

        int turn;

        public String GetAIName()
        {
            return "Noob AI";
        }

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;

            woodHexa = 0;
            cornHexa = 0;
            meatHexa = 0;
            oreHexa = 0;
            stoneHexa = 0;
            turn = 0;
        }

        public void ResolveAI()
        {
            if (mapController.GetState() == EGameState.StateFirstTown)
            {
                BuildTown(FindBestFreeTownID());
            }
            else if (mapController.GetState() == EGameState.StateSecondTown)
            {
                BuildTown(FindBestFreeTownID());
            }
            else
            {
                turn++;
                BuildRandomTown();
                if(turn % 5 == 0)
                    BuildRandomRoad();
            }
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new NoobAI();
            return component;
        }

        public void BuildRandomTown()
        {
            int maxTownID = mapController.GetMaxTownID();

            for (int loop1 = maxTownID; loop1 > 0; loop1--)
            {
                ITownGet town = mapController.GetITownGetByID(loop1);

                if (town.CanActivePlayerBuildTown() == TownBuildError.OK)
                {
                    mapController.BuildTown(loop1);
                    break;
                }
            }
        }

        public void BuildRandomRoad()
        {
             int maxRoadID = mapController.GetMaxRoadID();

             for (int loop1 = maxRoadID; loop1 > 0; loop1--)
             {
                 IRoadGet road = mapController.GetIRoadGetByID(loop1);

                 if (road.CanActivePlayerBuildRoad() == RoadBuildError.OK)
                 {
                     mapController.BuildRoad(loop1);
                     break ;
                 }
             }
        }

        public void BuildTown(int id)
        {
            mapController.BuildTown(id);
            ITownGet town = mapController.GetITownGetByID(id);
            for (int loop2 = 0; loop2 < 3; loop2++)
            {
                IHexaGet hexa = town.getIHexaGet(loop2);
                if (hexa != null)
                {
                    switch(hexa.getType())
                    {
                        case HexaKind.Cornfield :
                            cornHexa += hexa.getCurrentSource();
                            break;
                        case HexaKind.Pasture :
                            meatHexa += hexa.getCurrentSource();
                            break;
                        case HexaKind.Stone:
                            stoneHexa += hexa.getCurrentSource();
                            break;
                        case HexaKind.Forest:
                            woodHexa += hexa.getCurrentSource();
                            break;
                        case HexaKind.Mountains:
                            oreHexa += hexa.getCurrentSource();
                            break;
                    }
                }
            }
        }


        public int FindBestFreeTownID()
        {
            int maxTownID = mapController.GetMaxTownID();
            int maxSum = 0;
            int maxSumTownID = -1;
            const float MULTI_WOOD = 2.0f;
            const float MULTI_ORE = 3.0f;
            const float MULTI_STONE = 3.0f;
            const float MULTI_MEAT = 1.8f;
            const float MULTI_CORN = 1.8f;

            for (int loop1 = 1; loop1 < maxTownID; loop1++)
            {
                int sourceSum = 0;
                ITownGet town = mapController.GetITownGetByID(loop1);

                if (town.CanActivePlayerBuildTown() != TownBuildError.OK)
                    continue;

                bool tempCorn, tempWood, tempMeat, tempOre, tempStone;
                tempCorn = false;
                tempWood = false;
                tempStone = false;
                tempOre = false;
                tempMeat = false;

                for (int loop2 = 0; loop2 < 3; loop2++)
                {
                    IHexaGet hexa = town.getIHexaGet(loop2);
                    if (hexa != null)
                    {
                        float multi = 1.0f;
                        switch (hexa.getType())
                        {
                            case HexaKind.Cornfield:
                                if (cornHexa == 0 && tempCorn == false)
                                {
                                    tempCorn = true;
                                    multi = MULTI_CORN;
                                }
                                break;
                            case HexaKind.Pasture:
                                if (meatHexa == 0 && tempMeat == false)
                                {
                                    tempMeat = true;
                                    multi = MULTI_MEAT;
                                }
                                break;
                            case HexaKind.Stone:
                                if (stoneHexa == 0 && tempStone == false)
                                {
                                    tempStone = true;
                                    multi = MULTI_STONE;
                                }
                                break;
                            case HexaKind.Forest:
                                if (woodHexa == 0 && tempWood == false)
                                {
                                    tempWood = true;
                                    multi = MULTI_WOOD;
                                }
                                break;
                            case HexaKind.Mountains:
                                if (oreHexa == 0 && tempOre == false)
                                {
                                    tempOre = true;
                                    multi = MULTI_ORE;
                                }
                                break;
                        }
                        sourceSum += (int) (hexa.getCurrentSource() * multi);
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
