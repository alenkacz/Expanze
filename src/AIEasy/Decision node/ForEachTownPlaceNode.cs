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

            for(int loop1 = 0; loop1 < towns.Count; loop1++)
            {
                tree.SetActiveObject(towns[loop1]);
                trueNode.Execute();

                if (tree.GetWasAction())
                    return;
            }

            falseNode.Execute();
        }
    }
}
