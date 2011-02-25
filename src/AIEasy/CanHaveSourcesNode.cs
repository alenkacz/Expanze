using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class CanHaveSourcesNode : DecisionBinaryNode
    {
        ISourceAll source;
        IMapController map;

        public CanHaveSourcesNode(ITreeNode trueNode, ITreeNode falseNode, PriceKind kind, IMapController map)
            : base(trueNode, falseNode, null)
        {
            this.source = map.GetPrice(kind);
            condition = Condition;
            this.map = map;
        }

        private bool Condition()
        {
            IPlayer player = map.GetPlayerMe();
            return map.CanChangeSourcesFor(source) >= 0;
        }
    }
}
