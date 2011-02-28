using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class ForEachTownPlaceNode : DecisionBinaryNode
    {
        DecisionTree tree;

        public ForEachTownPlaceNode(ITreeNode trueNode, ITreeNode falseNode, DecisionTree tree)
            : base(trueNode, falseNode, null)
        {
            this.tree = tree;
        }

        public override void Execute()
        {
            List<ITown> towns = tree.GetAI().GetFreeTownPlaces();

            foreach (ITown town in towns)
            {
                tree.SetActiveTown(town);
                trueNode.Execute();

                if (tree.GetWasAction())
                    return;
            }

            falseNode.Execute();
        }
    }
}
