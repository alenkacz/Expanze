﻿using System;
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

        List<ITown> freeTownPlaces;
        List<IRoad> freeRoadPlaces;
        List<ITown> towns;
        //List<IRoad> leaveRoads;
        int freeHexaInTown;
        int[] sourceNormal;        // how many sources player get for one turn
        int licenceAmount;         // how many licences player bought
        int upgradeAmount;         // how many upgrades player did

        DecisionTree decisionTree;

        public String GetAIName()
        {
            return "AI lehká";
        }

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;
            Fitness.SetMapController(mapController, this);

            freeTownPlaces = new List<ITown>();
            towns = new List<ITown>();
            freeRoadPlaces = new List<IRoad>();
            freeHexaInTown = 0;
            sourceNormal = new int[5];
            for (int loop1 = 0; loop1 < 5; loop1++)
                sourceNormal[loop1] = 0;

            licenceAmount = 0;
            upgradeAmount = 0;

            decisionTree = new DecisionTree(mapController, this);
        }

        public bool EmptyLeave()
        {
            /// should be used another leave/node
            return false;
        }

        public List<ITown> GetFreeTownPlaces() { return freeTownPlaces; }
        public List<ITown> GetTowns() { return towns; }
        public List<IRoad> GetFreeRoadPlaces() { return freeRoadPlaces; }
        public int[] GetSourceNormal() { return sourceNormal; }
        public bool IsFreeHexaInTown()
        {
            return freeHexaInTown > 0;
        }


        public bool BuildTown(int ID)
        {
            ITown town;
            IRoad road;

            town = mapController.BuildTown(ID);
            if (town == null)
            {
                town = mapController.BuildTown(ID);
            }

            if (town != null)
            {
                towns.Add(town);
                ClearBadTownPlaces();


                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    if (town.GetIHexa(loop1).GetKind() != HexaKind.Water && town.GetIHexa(loop1).GetKind() != HexaKind.Nothing)
                        freeHexaInTown++;

                    if (mapController.GetState() != EGameState.StateGame)
                    {
                        road = town.GetIRoad(loop1);
                        if (road != null) // There is water
                        {
                            freeRoadPlaces.Add(road);
                        }
                    }
                }
                return true;
            }
            else
            {
                freeTownPlaces.Remove(mapController.GetITownByID(ID));
                throw new Exception("Building town. " + mapController.GetLastError());
                //return false;
            }
        }

        internal bool BuildRoad(IRoad activeRoad)
        {
            if (activeRoad == null)
                return false;

            if (activeRoad.Build() != null)
            {
                freeRoadPlaces.Remove(activeRoad);

                foreach (ITown town in activeRoad.GetITown())
                {
                    if (town.GetIOwner() != mapController.GetPlayerMe() && town.GetIOwner() != null)
                        continue;

                    for (byte loop1 = 0; loop1 < 3; loop1++)
                    {
                        IRoad road = town.GetIRoad(loop1);

                        if (road != null)
                        {
                            RoadBuildError tempError = road.CanBuildRoad();
                            if (tempError == RoadBuildError.OK ||
                                tempError == RoadBuildError.NoSources)
                            {
                                if (!freeRoadPlaces.Contains(road))
                                {
                                    freeRoadPlaces.Add(road);
                                }
                            }
                        }
                    }

                    if (town.IsPossibleToBuildTown())
                    {
                        freeTownPlaces.Remove(town);
                        freeTownPlaces.Add(town);
                    }
                }
                return true;
            }
            else
            {
                freeRoadPlaces.Remove(activeRoad);
                throw new Exception("Buidling road. " + mapController.GetLastError());
                //return false;
            }
        }

        internal bool BuildSourceBuilding(ITown town, byte pos)
        {
            if (town.BuildSourceBuilding(pos))
            {
                freeHexaInTown--;

                int amount = town.GetIHexa(pos).GetStartSource();
                sourceNormal[(int) (town.GetIHexa(pos).GetKind())] += amount;

                return true;
            }

            throw new Exception("Buidling source building. " + mapController.GetLastError());
            //return false;
        }

        public void ResolveAI()
        {
            switch (mapController.GetState())
            {
                case EGameState.StateFirstTown :
                    BuildTown(GetBestTownPlace());
                    break;
                case EGameState.StateSecondTown :
                    BuildTown(GetBestTownPlace());
                    break;
                case EGameState.StateGame :
                    ClearBadTownPlaces();
                    ClearBadRoadPlaces();
                    decisionTree.SolveAI();
                    break;
            }
        }

        private int GetBestTownPlace()
        {
            float tempFitness;
            float maxFitness = -0.1f;
            int maxTownID = -1;
            
            ITown town;

            for (int loop1 = 1; loop1 <= mapController.GetMaxTownID(); loop1++)
            {
                town = mapController.GetITownByID(loop1);
                if ((tempFitness = Fitness.GetFitness(town)) > maxFitness)
                {
                    maxFitness = tempFitness;
                    maxTownID = town.GetTownID();
                }
            }

            return maxTownID;
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIEasy();
            return component;
        }

        private void ClearBadRoadPlaces()
        {
            bool wasClearing = true;
            while (wasClearing)
            {
                for (int loop1 = 0; loop1 < freeRoadPlaces.Count; loop1++)
                {
                    RoadBuildError tempError = freeRoadPlaces[loop1].CanBuildRoad();
                    if (tempError == RoadBuildError.AlreadyBuild ||
                        tempError == RoadBuildError.InvalidRoadID ||
                        tempError == RoadBuildError.NoPlayerRoadOrTown)
                    {
                        freeRoadPlaces.RemoveAt(loop1);
                        break;
                    }
                }

                wasClearing = false;
            }
        }

        private void ClearBadTownPlaces()
        {
            bool wasClearing = true;
            while (wasClearing)
            {
                for (int loop1 = 0; loop1 < freeTownPlaces.Count; loop1++)
                {
                    if (!freeTownPlaces[loop1].IsPossibleToBuildTown())
                    {
                        freeTownPlaces.RemoveAt(loop1);
                        break;
                    }
                }
                wasClearing = false;
            }
        }

        internal bool BuildMarket(ITown town, byte pos)
        {
            if (town.BuildMarket(pos) == null)
            {
                throw new Exception("Market should have been built. " + mapController.GetLastError());
                //return false;
            }
            else
            {
                freeHexaInTown--;
                return true;
            }
        }

        internal bool BuildMonastery(ITown town, byte pos)
        {
            if (town.BuildMonastery(pos) == null)
            {
                throw new Exception("Monastery should have been built. " + mapController.GetLastError());
                //return false;
            }
            else
            {
                freeHexaInTown--;
                return true;
            }
        }

        internal bool BuyLicence(SourceKind activeSourceKind)
        {
            if (mapController.BuyLicence(activeSourceKind))
            {
                licenceAmount++;
                return true;
            }
            else
            {
                throw new Exception("Buying licence. " + mapController.GetLastError());
                //return false;
            }
        }

        internal bool HasFreeSlotInMarket()
        {
            int maxSlot = 3 * mapController.GetPlayerMe().GetBuildingCount(Building.Market);

            return licenceAmount < maxSlot;
        }

        internal bool InventUpgrade(SourceBuildingKind activeSourceBuildingKind)
        {
            if (mapController.InventUpgrade(activeSourceBuildingKind))
            {
                upgradeAmount++;
                return true;
            }
            else
            {
                throw new Exception("Inventing upgrade. " + mapController.GetLastError());
                //return false;
            }
        }

        internal bool HasFreeSlotInMonastery()
        {
            int maxSlot = 3 * mapController.GetPlayerMe().GetBuildingCount(Building.Monastery);

            return upgradeAmount < maxSlot;
        }

        internal bool EveryTurnALotOfOneSource(int amountLimit)
        {
            ISourceAll source = mapController.GetPlayerMe().GetCollectSourcesNormal();

            int max = 0;
            int temp;
            SourceKind maxKind = SourceKind.Count;

            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                temp = source.Get((SourceKind)loop1);
                if (temp > max)
                {
                    max = temp;
                    maxKind = (SourceKind)loop1;
                }
            }

            if (max >= amountLimit)
            {
                decisionTree.SetActiveObject(maxKind);
                switch (maxKind)
                {
                    case SourceKind.Corn: decisionTree.SetActiveObject(SourceBuildingKind.Mill); break;
                    case SourceKind.Meat: decisionTree.SetActiveObject(SourceBuildingKind.Stepherd); break;
                    case SourceKind.Stone: decisionTree.SetActiveObject(SourceBuildingKind.Quarry); break;
                    case SourceKind.Wood: decisionTree.SetActiveObject(SourceBuildingKind.Saw); break;
                    case SourceKind.Ore: decisionTree.SetActiveObject(SourceBuildingKind.Mine); break;
                }
                return true;
            }
            return false;
        }

    }
}
