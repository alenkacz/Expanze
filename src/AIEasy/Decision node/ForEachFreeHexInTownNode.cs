using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class ForEachFreeHexInTownNode : DecisionBinaryNode
    {
        DecisionTree tree;

        public ForEachFreeHexInTownNode(ITreeNode trueNode, ITreeNode falseNode, DecisionTree tree)
            : base(trueNode, falseNode, null)
        {
            this.tree = tree;
        }

        public override void Execute()
        {
            List<ITown> towns = tree.GetAI().GetTowns();
            HexaKind hexaKind;

            foreach (ITown town in towns)
            {
                tree.SetActiveTown(town);

                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    hexaKind = town.GetIHexa(loop1).GetKind();
                    if (hexaKind != HexaKind.Water &&
                        hexaKind != HexaKind.Nothing &&
                        hexaKind != HexaKind.Null &&
                        town.GetBuildingKind(loop1) == BuildingKind.NoBuilding)
                    {
                        tree.SetActivePosInTown(loop1);
                        
                        trueNode.Execute();

                        if (tree.GetWasAction())
                            return;
                    }
                }
            }

            falseNode.Execute();
        }
    }
}
