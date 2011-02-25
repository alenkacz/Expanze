using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class DecisionBinaryNode : DecisionNode
    {
        private ITreeNode trueNode;
        private ITreeNode falseNode;
        protected Condition condition;

        public DecisionBinaryNode(ITreeNode trueNode, ITreeNode falseNode, Condition condition)
        {
            this.trueNode = trueNode;
            this.falseNode = falseNode;
            this.condition = condition;
        }

        public override ITreeNode GetBranch()
        {
            if (condition())
                return trueNode;
            else
                return falseNode;
        }
    }
}
