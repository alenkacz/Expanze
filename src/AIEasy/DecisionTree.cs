using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using AIEasy.Action_node;
using CorePlugin.Action;

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

        List<ActionPrice> actionList;

        public DecisionTree(IMapController map, AIEasy ai)
        {
            actionList = new List<ActionPrice>();

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
            ActionPriceNode actionTownPrice = new ActionPriceNode(ActionKind.BuildTown, this);

            HaveSourcesNode haveSourcesForTown = new HaveSourcesNode(actionBuildTown, actionTownPrice /* back to for each */, PriceKind.BTown, map);
            ForEachTownPlaceNode localRoot = new ForEachTownPlaceNode(haveSourcesForTown, falseNode, this);

            return localRoot;
        }

        public ITreeNode MakeBuildSourceBuildingTree(ITreeNode falseNode)
        {
            ActionNode actionBuildSourceBuilding = new ActionNode(() => ai.BuildSourceBuilding(activeTown, activeTownPos), this);
            ActionPriceNode actionMillPrice = new ActionPriceNode(ActionKind.BuildMill, this);
            ActionPriceNode actionStepherdPrice = new ActionPriceNode(ActionKind.BuildStepherd, this);
            ActionPriceNode actionQuarryPrice = new ActionPriceNode(ActionKind.BuildQuarry, this);
            ActionPriceNode actionSawPrice = new ActionPriceNode(ActionKind.BuildSaw, this);
            ActionPriceNode actionMinePrice = new ActionPriceNode(ActionKind.BuildMine, this);

            DecisionBinaryNode isThatMill = new DecisionBinaryNode(actionMillPrice, actionStepherdPrice, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Cornfield; });
            DecisionBinaryNode isThatSaw = new DecisionBinaryNode(actionSawPrice, isThatMill, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Forest; });
            DecisionBinaryNode isThatQuarry = new DecisionBinaryNode(actionQuarryPrice, isThatSaw, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Stone; });
            DecisionBinaryNode isThatMine = new DecisionBinaryNode(actionMinePrice, isThatQuarry, () => { return activeTown.GetIHexa(activeTownPos).GetKind() == HexaKind.Mountains; });

            HaveSourcesNode haveSourcesForSourceBuilding = new HaveSourcesNode(actionBuildSourceBuilding, isThatMine, () => { return GetPriceForSourceBuilding(activeTown, activeTownPos); }, map);
            DecisionBinaryNode hasPlayerTargetSourceKind = new DecisionBinaryNode(haveSourcesForSourceBuilding, EA, () => { return ai.GetSourceNormal()[(int)activeTown.GetIHexa(activeTownPos).GetKind()] == 0; });
            DecisionBinaryNode localRoot = new DecisionBinaryNode(haveSourcesForSourceBuilding, hasPlayerTargetSourceKind, () => activeTown.GetIHexa(activeTownPos).GetStartSource() >= 12);

            return localRoot;
        }

        public ITreeNode MakeBuildRoadTree(ITreeNode falseNode)
        {
            ActionNode actionBuildRoad = new ActionNode(() => ai.BuildRoad(activeRoad), this);
            ActionPriceNode actionRoadPrice = new ActionPriceNode(ActionKind.BuildRoad, this);

            HaveSourcesNode hasSourcesForRoad = new HaveSourcesNode(actionBuildRoad, actionRoadPrice, PriceKind.BRoad, map);
            DecisionBinaryNode hasFreeTownPlace = new DecisionBinaryNode(hasSourcesForRoad, EA, () => { return ai.GetFreeTownPlaces().Count == 0; });
            ForEachRoadNode localRoot = new ForEachRoadNode(hasFreeTownPlace, falseNode, this);

            return localRoot;
        }

        public AIEasy GetAI() { return ai; }
        public void SetWasAction(bool action) { wasAction = action; }
        public bool GetWasAction() { return wasAction; }

        public void SetActiveTown(ITown town) { activeTown = town; }
        public ITown GetActiveTown() { return activeTown; }
        public void SetActiveRoad(IRoad road) { activeRoad = road; }
        public IRoad GetActiveRoad() { return activeRoad; }
        public void SetActiveObject(Object o)
        {
            if (o is ITown)
                SetActiveTown((ITown)o);
            else if (o is IRoad)
                SetActiveRoad((IRoad)o);
        }
        public void SetActivePosInTown(byte pos) { activeTownPos = pos; }

        public void SolveAI()
        {
            do
            {
                actionList.Clear();
                wasAction = false;
                root.Execute();
                if (!wasAction)
                {
                    TryMakeAction();
                }
            } while (wasAction);
        }

        public void AddActionPrice(ActionKind kind)
        {
            GameAction action = null;
            switch (kind)
            {
                case ActionKind.BuildRoad:
                    action = new BuildRoadAction(activeRoad.GetRoadID());
                    break;
                case ActionKind.BuildTown:
                    action = new BuildTownAction(activeTown.GetTownID());
                    break;
                case ActionKind.BuildMill:
                    action = new BuildMillAction(activeTown.GetTownID(), activeTownPos);
                    break;
                case ActionKind.BuildStepherd:
                    action = new BuildStepherdAction(activeTown.GetTownID(), activeTownPos);
                    break;
                case ActionKind.BuildQuarry:
                    action = new BuildQuarryAction(activeTown.GetTownID(), activeTownPos);
                    break;
                case ActionKind.BuildSaw:
                    action = new BuildSawAction(activeTown.GetTownID(), activeTownPos);
                    break;
                case ActionKind.BuildMine:
                    action = new BuildMineAction(activeTown.GetTownID(), activeTownPos);
                    break;
                default:
                    throw new Exception("No action definition for that action kind");
            }

            actionList.Add(new ActionPrice(action, map.CanChangeSourcesFor(map.GetPrice(action.GetPriceKind()))));
        }

        private void TryMakeAction()
        {
            if (actionList.Count > 0)
            {
                ActionPrice bestAction = null;
                int bestPrice = -1;

                foreach (ActionPrice action in actionList)
                {
                    int price = action.GetPrice();
                    if (price > bestPrice)
                    {
                        bestPrice = price;
                        bestAction = action;
                    }
                }

                //if (bestPrice < 200)
                {
                    map.ChangeSourcesFor(map.GetPrice(bestAction.GetSourcePrice()));
                    bestAction.Execute(map);
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
                case HexaKind.Forest: return PriceKind.BMill;
                case HexaKind.Mountains: return PriceKind.BMine;
            }

            return PriceKind.BMill; // error
        }
    }
}
