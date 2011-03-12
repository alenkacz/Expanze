using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class StochasticNodeMultiple : DecisionNode
    {
        List<ITreeNode> nodeList;
        ITreeNode backNode;
        int[] permutation;
        Random rnd;
        DecisionTree tree;

        public StochasticNodeMultiple(List<ITreeNode> nodeList, ITreeNode backNode, DecisionTree tree)
        {
            this.nodeList = nodeList;
            rnd = new Random();
            permutation = new int[nodeList.Count];
            this.tree = tree;
            this.backNode = backNode;
        }

        public override void Execute()
        {
            MakeRandomPermutation();

            for (int loop1 = 0; loop1 < permutation.Length; loop1++)
            {
                nodeList[permutation[loop1]].Execute();
                if (tree.GetWasAction())
                    return;
            }

            backNode.Execute();
        }

        private void MakeRandomPermutation()
        {
            for (int loop1 = 0; loop1 < permutation.Length; loop1++)
                permutation[loop1] = loop1;

            int pos1, pos2;
            int temp;
            int length = permutation.Length;
            for (int loop1 = 0; loop1 < length * 3; loop1++)
            {
                pos1 = rnd.Next() % length;
                pos2 = rnd.Next() % length;

                temp = permutation[pos1];
                permutation[pos1] = permutation[pos2];
                permutation[pos2] = temp;
            }
        }

        public override ITreeNode GetBranch()
        {
            return nodeList[rnd.Next() % nodeList.Count];
        }
    }
}
