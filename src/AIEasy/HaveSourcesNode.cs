using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class HaveSourcesNode : DecisionBinaryNode
    {
        ISourceAll source;
        IMapController map;

        public HaveSourcesNode(ITreeNode trueNode, ITreeNode falseNode, PriceKind kind, IMapController map)
            : base(trueNode, falseNode, null)
        {
            this.source = map.GetPrice(kind);
            condition = Condition;
            this.map = map;
        }

        private bool Condition()
        {
            IPlayer player = map.GetPlayerMe();
            return source.HasPlayerSources(player);
        }
    }
}
