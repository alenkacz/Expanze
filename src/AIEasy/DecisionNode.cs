using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class DecisionNode : TreeNode
    {
        private TreeNode trueNode;
        private TreeNode falseNode;
        private Condition condition;

        public DecisionNode(TreeNode trueNode, TreeNode falseNode, Condition condition)
        {
            this.trueNode = trueNode;
            this.falseNode = falseNode;
            this.condition = condition;
        }

        public void Execute()
        {
            GetBranch().Execute();
        }

        protected TreeNode GetBranch()
        {
            if (condition())
                return trueNode;
            else
                return falseNode;
        }
    }
}
