using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    delegate bool Condition();
    delegate PriceKind GetPrice();
    delegate bool Action();

    public interface ITreeNode
    {
        void Execute();
    }

    public abstract class DecisionNode : ITreeNode
    {
        public virtual void Execute()
        {
            GetBranch().Execute();
        }
        public abstract ITreeNode GetBranch();       
    }
}
