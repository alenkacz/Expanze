using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class CanHaveSourcesNode : DecisionBinaryNode
    {
        IMapController map;
        GetPrice getPrice;

        public CanHaveSourcesNode(ITreeNode trueNode, ITreeNode falseNode, PriceKind kind, IMapController map)
            : this(trueNode, falseNode, () => {return kind;}, map)
        {

        }

        public CanHaveSourcesNode(ITreeNode trueNode, ITreeNode falseNode, GetPrice getPrice, IMapController map)
            : base(trueNode, falseNode, null)
        {
            this.getPrice = getPrice;
            condition = Condition;
            this.map = map;
        }

        private bool Condition()
        {
            IPlayer player = map.GetPlayerMe();
            return map.CanChangeSourcesFor(map.GetPrice(getPrice())) >= 0;
        }
    }
}
