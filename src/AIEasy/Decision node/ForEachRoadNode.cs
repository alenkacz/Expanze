using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class ForEachRoadNode : DecisionBinaryNode
    {
        DecisionTree tree;

        public ForEachRoadNode(ITreeNode trueNode, ITreeNode falseNode, DecisionTree tree)
            : base(trueNode, falseNode, null)
        {
            this.tree = tree;
        }

        public override void Execute()
        {
            List<IRoad> roads = tree.GetAI().GetFreeRoadPlaces();

            /// Order roads according desirability

            foreach (IRoad road in roads)
            {
                tree.SetActiveRoad(road);
                trueNode.Execute();

                if (tree.GetWasAction())
                    return;
            }

            falseNode.Execute();
        }
    }
}
