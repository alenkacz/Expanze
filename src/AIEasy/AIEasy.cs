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
        List<ITown> towns;
        DecisionTree decisionTree;

        public String GetAIName()
        {
            return "AI - Easy";
        }

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;
            towns = new List<ITown>();
            decisionTree = new DecisionTree(mapController, this);
        }

        public void BuildSourceBuildingsInTowns()
        {
            foreach (ITown town in towns)
            {
                for(byte loop1 = 0; loop1 < 3; loop1++)
                    town.BuildSourceBuilding(loop1);
            }
        }

        public void BuildTown()
        {
            ITown tempTown;

            for (int loop1 = 1; loop1 < mapController.GetMaxTownID(); loop1 += 7)
            {
                tempTown = mapController.BuildTown(loop1);
                if (tempTown != null)
                {
                    towns.Add(tempTown);
                }
            }
        }

        public void ResolveAI()
        {
            switch (mapController.GetState())
            {
                case EGameState.StateFirstTown :
                    break;
                case EGameState.StateSecondTown :
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
