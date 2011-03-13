using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class ActionNode : ITreeNode
    {
        protected DelAction action;
        protected DecisionTree tree;

        public ActionNode(DelAction action, DecisionTree tree)
        {
            this.action = action;
            this.tree = tree;
        }

        public virtual void Execute()
        {        
            tree.SetWasAction(action());
        }
    }
}
