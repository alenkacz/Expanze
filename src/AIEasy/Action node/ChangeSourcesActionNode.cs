using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy.Action_node
{
    class ChangeSourcesActionNode : ActionNode
    {
        public ChangeSourcesActionNode(ActionSource action, DecisionTree tree)
            : base(() => { tree.AddActionSource(action); return false; }, tree)
        {
        }
    }
}
