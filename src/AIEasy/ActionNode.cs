using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class ActionNode : ITreeNode
    {
        Action action;
        public ActionNode(Action action)
        {
            this.action = action;
        }

        public void Execute()
        {
            action();
        }
    }
}
