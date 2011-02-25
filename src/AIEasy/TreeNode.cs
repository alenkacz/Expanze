using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    delegate bool Condition();
    delegate void Action();

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
