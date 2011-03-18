using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class SerialNode : DecisionNode
    {
        List<ITreeNode> nodeList;
        ITreeNode backNode;
        DecisionTree tree;

        public SerialNode(List<ITreeNode> nodeList, ITreeNode backNode, DecisionTree tree)
        {
            this.nodeList = nodeList;
            this.backNode = backNode;
            this.tree = tree;
        }

        public override void Execute()
        {
            foreach (ITreeNode node in nodeList)
            {
                node.Execute();

                if (tree.GetWasAction())
                    return;
            }

            GetBranch().Execute() ;
        }

        public override ITreeNode GetBranch()
        {
            return backNode;
        }
    }
}
