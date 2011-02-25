using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class DecisionTree
    {
        IMapController map;
        AIEasy ai;
        ITreeNode root;

        public DecisionTree(IMapController map, AIEasy ai)
        {
            this.map = map;
            this.ai = ai;

            BuildTree();
        }

        private void BuildTree()
        {
            ITreeNode actionBestTown = new ActionNode(ai.BuildTown);
            ITreeNode actionBuildSourceBuildings = new ActionNode(ai.BuildSourceBuildingsInTowns);

            root = new DecisionBinaryNode(actionBestTown, actionBuildSourceBuildings, () => { return map.GetState() == EGameState.StateSecondTown; });
        }

        public void SolveAI()
        {
            root.Execute();
        }
    }
}
