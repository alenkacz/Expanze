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
        bool hasFort;
        int fortTownID;
        int fortHexaID;
        bool hasMonastery;
        bool hasMarket;

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

            hasFort = false;

            hasMarket = false;
            hasMonastery = false;

            Random random = new Random();
            turn = random.Next() % 5;
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
                TryChangeSources();
                turn++;
                BuildAllPossibleSourceBuilding();
                BuildRandomTown();
                
                if (turn % 4 == 0)
                {
                    for (int loop1 = 0; loop1 < 3; loop1++)
                    {
                        if (BuildRandomRoad())
                        {
                            TryChangeSources();
                            if (BuildRandomTown())
                            {
                                TryChangeSources();
                                BuildAllPossibleSourceBuilding();
                            }
                        }
                    }
                }
                mapController.BuyUpgradeInSpecialBuilding(fortTownID, fortHexaID, UpgradeKind.FirstUpgrade, 3);
            }
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new NoobAI();
            return component;
        }

        private void TryChangeSources()
        {
            SourceKind minKind = SourceKind.Count, maxKind = SourceKind.Count;
            int min = 10000, max = -1;
            ISourceAll source = mapController.GetPlayerMe().GetSource();

            for(int loop1 = 0; loop1 < (int) SourceKind.Count; loop1++)
            {
                if (source[loop1] > max)
                {
                    max = source[loop1];
                    maxKind = source.IntToKind(loop1);
                }
                if (source[loop1] < min)
                {
                    min = source[loop1];
                    minKind = source.IntToKind(loop1);
                }
            }

            if (max > 200 && min < 100 || max > 800)
            {
                mapController.ChangeSources(maxKind, minKind, 100);
            }
        }

        public void BuildAllPossibleSourceBuilding()
        {
            int hexaID;
            ITownGet town;
            IHexaGet hexa;
            for (int loop1 = 1; loop1 < mapController.GetMaxTownID(); loop1++)
            {
                for (int loop2 = 0; loop2 < 3; loop2++)
                {
                    town = mapController.GetITownGetByID(loop1);
                    hexa = town.getIHexaGet(loop2);
                    hexaID = hexa.getID();

                    if (turn > 7)
                    {
                        if (hexa.getStartSource() <= 12)
                        {
                            if (!hasFort)
                            {
                                if (mapController.BuildBuildingInTown(loop1, hexaID, BuildingKind.FortBuilding) != null)
                                {
                                    hasFort = true;
                                    fortHexaID = hexaID;
                                    fortTownID = loop1;
                                }
                                continue;
                            }
                            else if (!hasMarket)
                            {
                                if (mapController.BuildBuildingInTown(loop1, hexaID, BuildingKind.MarketBuilding) != null)
                                {
                                    hasMarket = true;
                                }
                                continue;
                            } if (!hasMonastery)
                            {
                                if (mapController.BuildBuildingInTown(loop1, hexaID, BuildingKind.MonasteryBuilding) != null)
                                {
                                    hasMonastery = true;
                                }
                                continue;
                            }
                            
                        }
                    }
                    mapController.BuildBuildingInTown(loop1, hexaID, BuildingKind.SourceBuilding);
                }
            }
        }

        public bool BuildRandomTown()
        {
            int maxTownID = mapController.GetMaxTownID();

            for (int loop1 = maxTownID; loop1 > 0; loop1--)
            {
                if (mapController.CanBuildTown(loop1) == TownBuildError.OK)
                {
                    mapController.BuildTown(loop1);
                    return true;
                }
            }
            return false;
        }

        public bool BuildRandomRoad()
        {
             int maxRoadID = mapController.GetMaxRoadID();

             for (int loop1 = maxRoadID; loop1 > 0; loop1--)
             {
                 int roadID = loop1;
                 switch ((turn / 5) % 4)
                 {
                     case 0: roadID = loop1; break;
                     case 1: roadID = (loop1 + 50) % maxRoadID; break;
                     case 2: roadID = maxRoadID - loop1; break;
                     case 3: roadID = ((maxRoadID - loop1) + 30) % maxRoadID; break;
                 }
                 IRoadGet road = mapController.GetIRoadGetByID(roadID);

                 if (road != null && road.CanActivePlayerBuildRoad() == RoadBuildError.OK)
                 {
                     mapController.BuildRoad(roadID);
                     return true;
                 }
             }
             return false;
        }

        public void BuildTown(int id)
        {
            ITownGet town = mapController.BuildTown(id);

            for (int loop2 = 0; loop2 < 3; loop2++)
            {
                IHexaGet hexa = town.getIHexaGet(loop2);
                if (hexa != null)
                {
                    switch(hexa.getKind())
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
                
                if (mapController.CanBuildTown(loop1) != TownBuildError.OK)
                    continue;

                ITownGet town = mapController.GetITownGetByID(loop1);

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
                        switch (hexa.getKind())
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
