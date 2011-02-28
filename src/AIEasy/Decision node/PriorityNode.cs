using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIEasy
{
    class PriorityNode : DecisionNode
    {
        List<Object> objectList;
        ITreeNode branch;
        DecisionTree tree;

        public PriorityNode(ITreeNode branch, List<Object> objectList, DecisionTree tree)
        {
            this.objectList = objectList;
            this.branch = branch;
            this.tree = tree;
        }

        public override ITreeNode GetBranch()
        {
            float maxFitness = -0.1f;
            Object maxObject = null;
            
            float tempFitness;

            foreach (Object o in objectList)
            {
                tempFitness = Fitness.GetFitness(o);
                if (tempFitness > maxFitness)
                {
                    maxFitness = tempFitness;
                    maxObject = o;
                }
            }

            tree.SetActiveObject(maxObject);

            return branch;
        }
    }
}
