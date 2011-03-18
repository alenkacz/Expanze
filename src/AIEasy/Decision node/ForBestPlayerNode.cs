using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class ForBestPlayerNode : DecisionBinaryNode
    {
        DecisionTree tree;

        public ForBestPlayerNode(ITreeNode trueNode, ITreeNode falseNode, DecisionTree tree)
            : base(trueNode, falseNode, null)
        {
            this.tree = tree;
        }

        public override void Execute()
        {
            List<IPlayer> players = tree.GetMapController().GetPlayerOthers();

            /// Order roads according desirability

            float maxFitness = -0.1f;
            IPlayer maxObject = null;

            float tempFitness;

            foreach (IPlayer p in players)
            {
                tempFitness = Fitness.GetFitness(p);
                if (tempFitness > maxFitness)
                {
                    maxFitness = tempFitness;
                    maxObject = p;
                }
            }


            if (maxObject != null)
            {
                tree.SetActiveObject(maxObject);
                trueNode.Execute();

                if (tree.GetWasAction())
                    return;
            }

            falseNode.Execute();
        }
    }
}
