﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class HasSourcesNode : DecisionBinaryNode
    {
        GetPrice getPrice;
        IMapController map;

        public HasSourcesNode(ITreeNode trueNode, ITreeNode falseNode, PriceKind kind, IMapController map)
            : this(trueNode, falseNode, () => {return kind;}, map)
        {

        }

        public HasSourcesNode(ITreeNode trueNode, ITreeNode falseNode, GetPrice getPrice, IMapController map)
            : base(trueNode, falseNode, null)
        {
            this.getPrice = getPrice;
            condition = Condition;
            this.map = map;
        }

        private bool Condition()
        {
            IPlayer player = map.GetPlayerMe();
            return map.GetPrice(getPrice()).HasPlayerSources(player);
        }
    }
}
