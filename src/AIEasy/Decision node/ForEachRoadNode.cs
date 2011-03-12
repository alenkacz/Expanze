using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class ForEachRoadNode : DecisionBinaryNode
    {
        DecisionTree tree;

        public ForEachRoadNode(ITreeNode trueNode, ITreeNode falseNode, DecisionTree tree)
            : base(trueNode, falseNode, null)
        {
            this.tree = tree;
        }

        public override void Execute()
        {
            List<IRoad> roads = tree.GetAI().GetFreeRoadPlaces();

            /// Order roads according desirability

            float maxFitness = -0.1f;
            IRoad maxObject = null;

            float tempFitness;

            foreach (IRoad r in roads)
            {
                
                RoadBuildError error = r.CanBuildRoad();
                if (error == RoadBuildError.AlreadyBuild ||
                    error == RoadBuildError.InvalidRoadID ||
                    error == RoadBuildError.NoPlayerRoadOrTown)
                    continue;

                tempFitness = Fitness.GetFitness(r);
                if (tempFitness > maxFitness)
                {
                    maxFitness = tempFitness;
                    maxObject = r;
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
