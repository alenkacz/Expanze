using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class FreeHexa
    {
        public ITown town;
        public byte pos;
        public float fitness;

        public FreeHexa(ITown town, byte pos, float fitness)
        {
            this.town = town;
            this.pos = pos;
            this.fitness = fitness;
        }
    }

    class ForEachFreeHexInTownNode : DecisionBinaryNode
    {
        DecisionTree tree;
        bool sourceBuilding;

        public ForEachFreeHexInTownNode(ITreeNode trueNode, ITreeNode falseNode, bool sourceBuilding, DecisionTree tree)
            : base(trueNode, falseNode, null)
        {
            this.tree = tree;
            this.sourceBuilding = sourceBuilding;
        }

        public override void Execute()
        {
            List<ITown> towns = tree.GetAI().GetTowns();
            List<FreeHexa> freeHexa = new List<FreeHexa>();

            HexaKind hexaKind;
            IHexa hexa;

            foreach (ITown town in towns)
            {
                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    hexa = town.GetIHexa(loop1);
                    hexaKind = hexa.GetKind();
                    if (hexaKind != HexaKind.Water &&
                        hexaKind != HexaKind.Nothing &&
                        hexaKind != HexaKind.Null &&
                        town.GetBuildingKind(loop1) == BuildingKind.NoBuilding)
                    {
                        freeHexa.Add(new FreeHexa(town, loop1, Fitness.GetFitness(hexa)));
                    }
                }
            }

            int multiply = (sourceBuilding) ? -1 : 1;

            freeHexa.Sort(delegate(FreeHexa a, FreeHexa b) {
                float delta = (a.fitness - b.fitness) * multiply;

                if (delta > 0)
                    return 1;
                else if (delta < 0)
                    return -1;
                else
                    return 0;
            });

            foreach (FreeHexa fh in freeHexa)
            {
                tree.SetActiveObject(fh.town);
                tree.SetActivePosInTown(fh.pos);

                trueNode.Execute();

                if (tree.GetWasAction())
                    return;
            }


            falseNode.Execute();
        }
    }
}
