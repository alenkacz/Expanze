using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class DecisionTree
    {
        IMapController map;
        AIEasy ai;

        ITreeNode root;
        FakeActionNode EA;           // empty action, not defined yet

        bool wasAction;

        ITown activeTown;
        byte activeTownPos;

        public DecisionTree(IMapController map, AIEasy ai)
        {
            this.map = map;
            this.ai = ai;

            BuildTree();
        }

        private void BuildTree()
        {
            EA = new FakeActionNode(ai.EmptyLeave, this);

            CanHaveSourcesNode canHaveSourceForSourceBuilding = new CanHaveSourcesNode(MakeBuildSourceBuildingTree(EA), EA, () => { return GetPriceForSourceBuilding(activeTown, activeTownPos); }, map);
            DecisionBinaryNode isThatHexaDesert = new DecisionBinaryNode(EA /* special building */, canHaveSourceForSourceBuilding, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Desert; });
            
            ForEachFreeHexInTownNode forEachFreeHexaInTown = new ForEachFreeHexInTownNode(isThatHexaDesert, EA /* build road */, this);
            DecisionBinaryNode hasFreeHexaInTown = new DecisionBinaryNode(forEachFreeHexaInTown, EA /* build road */, ai.IsFreeHexaInTown);
            root = new CanHaveSourcesNode(MakeBuildTownTree(hasFreeHexaInTown), hasFreeHexaInTown, PriceKind.BTown, map);
        }

        public ITreeNode MakeBuildTownTree(ITreeNode falseNode)
        {
            ITreeNode actionBuildTown = new ActionNode(() => ai.BuildTown(activeTown.GetTownID()), this);

            HaveSourcesNode haveSourcesForTown = new HaveSourcesNode(actionBuildTown, EA /* back to for each */, PriceKind.BTown, map);
            ForEachTownPlaceNode localRoot = new ForEachTownPlaceNode(haveSourcesForTown, falseNode, this);

            return localRoot;
        }

        public ITreeNode MakeBuildSourceBuildingTree(ITreeNode falseNode)
        {
            ActionNode actionBuildSourceBuilding = new ActionNode(() => ai.BuildSourceBuilding(activeTown, activeTownPos), this);

            HaveSourcesNode haveSourcesForSourceBuilding = new HaveSourcesNode(actionBuildSourceBuilding, EA, () => { return GetPriceForSourceBuilding(activeTown, activeTownPos); }, map);
            DecisionBinaryNode hasPlayerTargetSourceKind = new DecisionBinaryNode(haveSourcesForSourceBuilding, EA, () => {return ai.GetSourceNormal()[(int) activeTown.GetIHexa(activeTownPos).GetKind()] == 0;});
            DecisionBinaryNode localRoot = new DecisionBinaryNode(haveSourcesForSourceBuilding, hasPlayerTargetSourceKind, () => activeTown.GetIHexa(activeTownPos).GetStartSource() >= 12);

            return localRoot;
        }

        public AIEasy GetAI() { return ai; }
        public void SetWasAction(bool action) { wasAction = action; }
        public bool GetWasAction() { return wasAction; }

        public void SetActiveTown(ITown town) { activeTown = town; }
        public ITown GetActiveTown() { return activeTown; }

        public void SetActivePosInTown(byte pos) { activeTownPos = pos; }

        public void SolveAI()
        {
            do
            {
                wasAction = false;
                root.Execute();
                if (!wasAction)
                {
                    // zkus vymenit suroviny
                }
            } while (wasAction);
        }

        public PriceKind GetPriceForSourceBuilding(ITown town, byte pos)
        {
            HexaKind hexaKind = town.GetIHexa(pos).GetKind();
            switch (hexaKind)
            {
                case HexaKind.Cornfield: return PriceKind.BMill;
                case HexaKind.Pasture: return PriceKind.BStepherd;
                case HexaKind.Stone: return PriceKind.BQuarry;
                case HexaKind.Forest: return PriceKind.BMill;
                case HexaKind.Mountains: return PriceKind.BMine;
            }

            return PriceKind.BMill; // error
        }
    }
}
