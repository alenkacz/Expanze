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
        List<ITown> towns;
        List<IRoad> leaveRoads;
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

            freeTownPlaces = new List<ITown>();
            towns = new List<ITown>();
            leaveRoads = new List<IRoad>();
            freeHexaInTown = 0;
            sourceNormal = new int[5];
            for (int loop1 = 0; loop1 < 5; loop1++)
                sourceNormal[loop1] = 0;

            decisionTree = new DecisionTree(mapController, this);
        }

        public void EmptyLeave()
        {
            /// should be used another leave/node
        }

        public List<ITown> GetFreeTownPlaces() { return freeTownPlaces; }
        public List<ITown> GetTowns() { return towns; }
        public int[] GetSourceNormal() { return sourceNormal; }
        public bool IsFreeHexaInTown()
        {
            return freeHexaInTown > 0;
        }

        public void BuildTown(int ID)
        {
            ITown town;

            town = mapController.BuildTown(ID);

            if (town != null)
            {
                towns.Add(town);
                for(byte loop1 = 0; loop1 < 3; loop1++)
                {
                    if (town.GetIHexa(loop1).GetKind() != HexaKind.Water && town.GetIHexa(loop1).GetKind() != HexaKind.Nothing)
                        freeHexaInTown++;
                }
            }
        }

        internal void BuildSourceBuilding(ITown activeTown, byte activeTownPos)
        {
            if (activeTown.BuildSourceBuilding(activeTownPos))
            {
                freeHexaInTown--;

                int amount = activeTown.GetIHexa(activeTownPos).GetStartSource();
                sourceNormal[(int) (activeTown.GetIHexa(activeTownPos).GetKind())] += amount;
            }
        }

        public void ResolveAI()
        {
            switch (mapController.GetState())
            {
                case EGameState.StateFirstTown :
                    BuildTown(15);
                    break;
                case EGameState.StateSecondTown :
                    BuildTown(35);
                    break;
                case EGameState.StateGame :
                    decisionTree.SolveAI();
                    break;
            }
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIEasy();
            return component;
        }
    }
}
