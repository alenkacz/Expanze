using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class FakeActionNode : ActionNode
    {
        public FakeActionNode(DelAction action, DecisionTree tree) : base(action, tree)
        {
        }

        public override void Execute()
        {
            base.Execute();
            tree.SetWasAction(false);
        }
    }
}
