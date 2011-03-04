using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using AIEasy.Action_node;

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
        IRoad activeRoad;
        byte activeTownPos;

        List<ActionSource> actionSource;

        public DecisionTree(IMapController map, AIEasy ai)
        {
            actionSource = new List<ActionSource>();

            this.map = map;
            this.ai = ai;

            BuildTree();
        }

        private void BuildTree()
        {
            EA = new FakeActionNode(ai.EmptyLeave, this);

            CanHaveSourcesNode canHaveSourceForRoad = new CanHaveSourcesNode(MakeBuildRoadTree(EA), EA, PriceKind.BRoad, map);

            CanHaveSourcesNode canHaveSourceForSourceBuilding = new CanHaveSourcesNode(MakeBuildSourceBuildingTree(EA), EA, () => { return GetPriceForSourceBuilding(activeTown, activeTownPos); }, map);
            DecisionBinaryNode isThatHexaDesert = new DecisionBinaryNode(EA /* special building */, canHaveSourceForSourceBuilding, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Desert; });

            ForEachFreeHexInTownNode forEachFreeHexaInTown = new ForEachFreeHexInTownNode(isThatHexaDesert, canHaveSourceForRoad, this);
            DecisionBinaryNode hasFreeHexaInTown = new DecisionBinaryNode(forEachFreeHexaInTown, canHaveSourceForRoad, ai.IsFreeHexaInTown);
            root = new CanHaveSourcesNode(MakeBuildTownTree(hasFreeHexaInTown), hasFreeHexaInTown, PriceKind.BTown, map);
        }

        public ITreeNode MakeBuildTownTree(ITreeNode falseNode)
        {
            ITreeNode actionBuildTown = new ActionNode(() => ai.BuildTown(activeTown.GetTownID()), this);
            ChangeSourcesActionNode addActionTown = new ChangeSourcesActionNode(new ActionSource(() => ai.BuildTown(activeTown.GetTownID()), PriceKind.BTown), this);

            HaveSourcesNode haveSourcesForTown = new HaveSourcesNode(actionBuildTown, addActionTown /* back to for each */, PriceKind.BTown, map);
            ForEachTownPlaceNode localRoot = new ForEachTownPlaceNode(haveSourcesForTown, falseNode, this);

            return localRoot;
        }

        public ITreeNode MakeBuildSourceBuildingTree(ITreeNode falseNode)
        {
            DelAction buildSourceBuilding = () => ai.BuildSourceBuilding(activeTown, activeTownPos);
            ActionNode actionBuildSourceBuilding = new ActionNode(buildSourceBuilding, this);
            ChangeSourcesActionNode addMillAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BMill), this);
            ChangeSourcesActionNode addStepherdAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BStepherd), this);
            ChangeSourcesActionNode addQuarryAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BQuarry), this);
            ChangeSourcesActionNode addSawAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BSaw), this);
            ChangeSourcesActionNode addMineAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BMine), this);

            DecisionBinaryNode isThatMill = new DecisionBinaryNode(addMillAction, addStepherdAction, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Cornfield; });
            DecisionBinaryNode isThatSaw = new DecisionBinaryNode(addSawAction, isThatMill, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Forest; });
            DecisionBinaryNode isThatQuarry = new DecisionBinaryNode(addQuarryAction, isThatSaw, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Stone; });
            DecisionBinaryNode isThatMine = new DecisionBinaryNode(addMineAction, isThatQuarry, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Mountains; });

            HaveSourcesNode haveSourcesForSourceBuilding = new HaveSourcesNode(actionBuildSourceBuilding, isThatMine, () => { return GetPriceForSourceBuilding(activeTown, activeTownPos); }, map);
            DecisionBinaryNode hasPlayerTargetSourceKind = new DecisionBinaryNode(haveSourcesForSourceBuilding, EA, () => { return ai.GetSourceNormal()[(int)activeTown.GetIHexa(activeTownPos).GetKind()] == 0; });
            DecisionBinaryNode localRoot = new DecisionBinaryNode(haveSourcesForSourceBuilding, hasPlayerTargetSourceKind, () => activeTown.GetIHexa(activeTownPos).GetStartSource() >= 12);

            return localRoot;
        }

        public ITreeNode MakeBuildRoadTree(ITreeNode falseNode)
        {
            ActionNode actionBuildRoad = new ActionNode(() => ai.BuildRoad(activeRoad), this);
            ChangeSourcesActionNode addActionRoad = new ChangeSourcesActionNode(new ActionSource(() => ai.BuildRoad(activeRoad), PriceKind.BRoad), this);
            
            HaveSourcesNode hasSourcesForRoad = new HaveSourcesNode(actionBuildRoad, addActionRoad, PriceKind.BRoad, map);
            DecisionBinaryNode hasFreeTownPlace = new DecisionBinaryNode(hasSourcesForRoad, EA, () => { return ai.GetFreeTownPlaces().Count == 0; });
            ForEachRoadNode localRoot = new ForEachRoadNode(hasFreeTownPlace, falseNode, this);

            return localRoot;
        }

        public AIEasy GetAI() { return ai; }
        public void SetWasAction(bool action) { wasAction = action; }
        public bool GetWasAction() { return wasAction; }

        public void SetActiveObject(ITown town) { activeTown = town; }
        public ITown GetActiveTown() { return activeTown; }
        public void SetActiveObject(IRoad road) { activeRoad = road; }
        public IRoad GetActiveRoad() { return activeRoad; }

        public void SetActivePosInTown(byte pos) { activeTownPos = pos; }

        public void SolveAI()
        {
            do
            {
                actionSource.Clear();
                wasAction = false;
                root.Execute();
                if (!wasAction)
                {
                    TryMakeAction();
                }
            } while (wasAction);
        }

        public void AddActionSource(ActionSource action)
        {
            if (map.CanChangeSourcesFor(map.GetPrice(action.GetPriceKind())) < 0)
            {
                int a = 5;
                a++;
            }
            action.SetSourceChange(map.CanChangeSourcesFor(map.GetPrice(action.GetPriceKind())));
            actionSource.Add(action);
        }

        private void TryMakeAction()
        {
            if (actionSource.Count > 0)
            {
                ActionSource bestAction = null;
                int bestPrice = -1;

                foreach (ActionSource action in actionSource)
                {
                    int price = action.GetSourceChange();
                    if (price > bestPrice)
                    {
                        bestPrice = price;
                        bestAction = action;
                    }
                }

                if (map == null || bestAction == null)
                {
                    int a = 6;
                    a++;
                }
                {
                    map.ChangeSourcesFor(map.GetPrice(bestAction.GetPriceKind()));
                    bestAction.GetAction()();
                    SetWasAction(true);
                }
            }
        }

        public PriceKind GetPriceForSourceBuilding(ITown town, byte pos)
        {
            HexaKind hexaKind = town.GetIHexa(pos).GetKind();
            switch (hexaKind)
            {
                case HexaKind.Cornfield: return PriceKind.BMill;
                case HexaKind.Pasture: return PriceKind.BStepherd;
                case HexaKind.Stone: return PriceKind.BQuarry;
                case HexaKind.Forest: return PriceKind.BSaw;
                case HexaKind.Mountains: return PriceKind.BMine;
            }

            return PriceKind.BMill; // error
        }
    }
}
