﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class StochasticNodeOne : DecisionNode
    {
        List<ITreeNode> nodeList;
        Random rnd;

        public StochasticNodeOne(List<ITreeNode> nodeList)
        {
            this.nodeList = nodeList;
            rnd = new Random();
        }

        public override ITreeNode GetBranch()
        {
            return nodeList[rnd.Next() % nodeList.Count];
        }
    }
}