using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class ForBestFortHexaNeighbourNode : DecisionBinaryNode
    {
        DecisionTree tree;

        public ForBestFortHexaNeighbourNode(ITreeNode trueNode, ITreeNode falseNode, DecisionTree tree)
            : base(trueNode, falseNode, null)
        {
            this.tree = tree;
        }

        public override void Execute()
        {
            IMapController map = tree.GetMapController();
            List<IFort> forts = map.GetPlayerMe().GetFort();

            /// Order roads according desirability

            float maxFitness = -0.1f;
            IHexa maxObject = null;

            float tempFitness;
            IHexa hexa;

            foreach (IFort fort in forts)
            {
                hexa = map.GetIHexaByID(fort.GetHexaID());

                for (int loop1 = 0; loop1 < 6; loop1++)
                {
                    tempFitness = Fitness.GetFitness(hexa.GetIHexaNeighbour((RoadPos) loop1));
                    if (tempFitness > maxFitness)
                    {
                        maxFitness = tempFitness;
                        maxObject = hexa.GetIHexaNeighbour((RoadPos)loop1);
                    }
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
