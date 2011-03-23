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
            IPlayer me = map.GetPlayerMe();
            List<IFort> forts = me.GetFort();

            /// Order roads according desirability

            float maxFitness = -0.1f;
            IHexa maxObject = null;

            float tempFitness;
            IHexa hexa;
            IHexa hexaNeighbour;
            foreach (IFort fort in forts)
            {
                hexa = map.GetIHexaByID(fort.GetHexaID());

                if (hexa == null)
                    continue;

                for (int loop1 = 0; loop1 < 6; loop1++)
                {                    
                    hexaNeighbour = hexa.GetIHexaNeighbour((RoadPos) loop1);
                    if (hexaNeighbour == null)
                        continue;

                    tempFitness = Fitness.GetFitness(me, hexaNeighbour);
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
