using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class ActionNode : ITreeNode
    {
        protected Action action;
        protected DecisionTree tree;

        public ActionNode(Action action, DecisionTree tree)
        {
            this.action = action;
            this.tree = tree;
        }

        public virtual void Execute()
        {
            action();
            tree.SetWasAction(true);
        }
    }
}
