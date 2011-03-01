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

        List<ITown> freeTownPlaces;
        List<IRoad> freeRoadPlaces;
        List<ITown> towns;
        //List<IRoad> leaveRoads;
        int freeHexaInTown;
        int[] sourceNormal;        // how many sources player get for one turn

        DecisionTree decisionTree;

        public String GetAIName()
        {
            return "AI - Easy";
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

            if (town != null)
            {
                towns.Add(town);
                freeTownPlaces.Remove(town);

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
                freeTownPlaces.Remove(town);
                return false;
            }
        }

        internal bool BuildRoad(IRoad activeRoad)
        {
            if (activeRoad.Build() != null)
            {
                freeRoadPlaces.Remove(activeRoad);

                foreach (ITown town in activeRoad.GetITown())
                {
                    for (byte loop1 = 0; loop1 < 3; loop1++)
                    {
                        IRoad road = town.GetIRoad(loop1);
                        if (road != null &&
                            !road.GetIsBuild() &&
                            !freeRoadPlaces.Contains(road))
                        {
                            freeRoadPlaces.Add(road);
                        }
                    }

                    if(town.IsPossibleToBuildTown())
                        freeTownPlaces.Add(town);
                }
                return true;
            }
            else
            {
                freeRoadPlaces.Remove(activeRoad);
                return false;
            }
        }

        internal bool BuildSourceBuilding(ITown activeTown, byte activeTownPos)
        {
            if (activeTown.BuildSourceBuilding(activeTownPos))
            {
                freeHexaInTown--;

                int amount = activeTown.GetIHexa(activeTownPos).GetStartSource();
                sourceNormal[(int) (activeTown.GetIHexa(activeTownPos).GetKind())] += amount;

                return true;
            }
            return false;
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
    }
}
