using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    delegate bool Condition();
    delegate void Action();

    interface TreeNode
    {
        void Execute();
    }
}
