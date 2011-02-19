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

        public String GetAIName()
        {
            return "AI - Easy";
        }

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;
            towns = new List<ITown>();
            MakeDecisionTree();
        }

        TreeNode root;
        private void MakeDecisionTree()
        {
            TreeNode actionBestTown = new ActionNode(BuildTown);
            TreeNode actionBuildSourceBuildings = new ActionNode(BuildSourceBuildingsInTowns);
            TreeNode stateFirstTown;
            TreeNode state23;
            TreeNode stateSecondTown;
            TreeNode stateGame;

            state23 = new DecisionNode(actionBestTown, actionBuildSourceBuildings, () => { return mapController.GetState() == EGameState.StateSecondTown; });
            root = new DecisionNode(actionBestTown, state23, () => { return mapController.GetState() == EGameState.StateFirstTown; });
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
            root.Execute();
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIEasy();
            return component;
        }     
    }
}
