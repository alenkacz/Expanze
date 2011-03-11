﻿using System;
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

        ActiveState activeState;

        List<ActionSource> actionSource;

        public DecisionTree(IMapController map, AIEasy ai)
        {
            actionSource = new List<ActionSource>();
            activeState = new ActiveState();

            this.map = map;
            this.ai = ai;

            BuildTree();
        }

        private void BuildTree()
        {
            EA = new FakeActionNode(ai.EmptyLeave, this);

            ITreeNode marketTree = MakeBuyLicenceTree(EA);
            CanHaveSourcesNode canHaveSourceForRoad = new CanHaveSourcesNode(MakeBuildRoadTree(marketTree), marketTree, PriceKind.BRoad, map);

            List<ITreeNode> specialBuildingList;
            specialBuildingList = new List<ITreeNode>();
            specialBuildingList.Add(MakeBuildMarketTree());

            StochasticNodeMultiple specialBuilding = new StochasticNodeMultiple(specialBuildingList);
            DecisionBinaryNode canBuildSpecialBuilding = new DecisionBinaryNode(specialBuilding, EA, () => { return activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind() != HexaKind.Mountains; });
            CanHaveSourcesNode canHaveSourceForSourceBuilding = new CanHaveSourcesNode(MakeBuildSourceBuildingTree(canBuildSpecialBuilding), canBuildSpecialBuilding, () => { return GetPriceForSourceBuilding(activeState.activeTown, activeState.activeTownPos); }, map);
            DecisionBinaryNode isThatHexaDesert = new DecisionBinaryNode(canBuildSpecialBuilding, canHaveSourceForSourceBuilding, () => { return activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind() == HexaKind.Desert; });

            ForEachFreeHexInTownNode forEachFreeHexaInTown = new ForEachFreeHexInTownNode(isThatHexaDesert, canHaveSourceForRoad, this);
            DecisionBinaryNode hasFreeHexaInTown = new DecisionBinaryNode(forEachFreeHexaInTown, canHaveSourceForRoad, ai.IsFreeHexaInTown);
            root = new CanHaveSourcesNode(MakeBuildTownTree(hasFreeHexaInTown), hasFreeHexaInTown, PriceKind.BTown, map);
        }

        public ITreeNode MakeBuildTownTree(ITreeNode falseNode)
        {
            ITreeNode actionBuildTown = new ActionNode(() => ai.BuildTown(activeState.activeTown.GetTownID()), this);
            ChangeSourcesActionNode addActionTown = new ChangeSourcesActionNode(new ActionSource(() => ai.BuildTown(activeState.activeTown.GetTownID()), PriceKind.BTown), this);

            HaveSourcesNode haveSourcesForTown = new HaveSourcesNode(actionBuildTown, addActionTown /* back to for each */, PriceKind.BTown, map);
            ForEachTownPlaceNode localRoot = new ForEachTownPlaceNode(haveSourcesForTown, falseNode, this);

            return localRoot;
        }

        public ITreeNode MakeBuildSourceBuildingTree(ITreeNode falseNode)
        {
            DelAction buildSourceBuilding = () => ai.BuildSourceBuilding(activeState.activeTown, activeState.activeTownPos);
            ActionNode actionBuildSourceBuilding = new ActionNode(buildSourceBuilding, this);
            ChangeSourcesActionNode addMillAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BMill), this);
            ChangeSourcesActionNode addStepherdAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BStepherd), this);
            ChangeSourcesActionNode addQuarryAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BQuarry), this);
            ChangeSourcesActionNode addSawAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BSaw), this);
            ChangeSourcesActionNode addMineAction = new ChangeSourcesActionNode(new ActionSource(buildSourceBuilding, PriceKind.BMine), this);

            DecisionBinaryNode isThatMill = new DecisionBinaryNode(addMillAction, addStepherdAction, () => { return activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind() == HexaKind.Cornfield; });
            DecisionBinaryNode isThatSaw = new DecisionBinaryNode(addSawAction, isThatMill, () => { return activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind() == HexaKind.Forest; });
            DecisionBinaryNode isThatQuarry = new DecisionBinaryNode(addQuarryAction, isThatSaw, () => { return activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind() == HexaKind.Stone; });
            DecisionBinaryNode isThatMine = new DecisionBinaryNode(addMineAction, isThatQuarry, () => { return activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind() == HexaKind.Mountains; });

            HaveSourcesNode haveSourcesForSourceBuilding = new HaveSourcesNode(actionBuildSourceBuilding, isThatMine, () => { return GetPriceForSourceBuilding(activeState.activeTown, activeState.activeTownPos); }, map);
            DecisionBinaryNode hasPlayerTargetSourceKind = new DecisionBinaryNode(haveSourcesForSourceBuilding, EA, () => { return ai.GetSourceNormal()[(int)activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind()] == 0; });
            DecisionBinaryNode localRoot = new DecisionBinaryNode(haveSourcesForSourceBuilding, hasPlayerTargetSourceKind, () => activeState.activeTown.GetIHexa(activeState.activeTownPos).GetStartSource() >= 12);

            return localRoot;
        }

        private ITreeNode MakeBuildRoadTree(ITreeNode falseNode)
        {
            ActionNode actionBuildRoad = new ActionNode(() => ai.BuildRoad(activeState.activeRoad), this);
            ChangeSourcesActionNode addActionRoad = new ChangeSourcesActionNode(new ActionSource(() => ai.BuildRoad(activeState.activeRoad), PriceKind.BRoad), this);
            
            HaveSourcesNode hasSourcesForRoad = new HaveSourcesNode(actionBuildRoad, addActionRoad, PriceKind.BRoad, map);
            DecisionBinaryNode hasFreeTownPlace = new DecisionBinaryNode(hasSourcesForRoad, EA, () => { return ai.GetFreeTownPlaces().Count == 0; });
            ForEachRoadNode localRoot = new ForEachRoadNode(hasFreeTownPlace, falseNode, this);

            return localRoot;
        }

        private ITreeNode MakeBuildMarketTree()
        {
            ActionNode actionBuildMarket = new ActionNode(() => ai.BuildMarket(activeState.activeTown, activeState.activeTownPos), this);
            ChangeSourcesActionNode addActionMarket = new ChangeSourcesActionNode(new ActionSource(() => ai.BuildMarket(activeState.activeTown, activeState.activeTownPos), PriceKind.BMarket), this);

            HaveSourcesNode haveSourcesForMarket = new HaveSourcesNode(actionBuildMarket, addActionMarket, PriceKind.BMarket, map);
            DecisionBinaryNode hasMarketWithFreeSlot = new DecisionBinaryNode(EA, haveSourcesForMarket, ai.HasFreeSlotInMarket);
            DecisionBinaryNode everyTurnALotOfOneSource = new DecisionBinaryNode(hasMarketWithFreeSlot, EA, () => { return ai.EveryTurnALotOfOneSource(48); });
            CanHaveSourcesNode canHaveSourcesForMarket = new CanHaveSourcesNode(everyTurnALotOfOneSource, EA, PriceKind.BMarket, map);

            return canHaveSourcesForMarket;
        }

        private ITreeNode MakeBuyLicenceTree(ITreeNode falseNode)
        {
            ActionNode actionBuyLicence = new ActionNode(() => ai.BuyLicence(activeState.activeSourceKind), this);
            //ChangeSourcesActionNode addActionBuyLicence = new ChangeSourcesActionNode(new ActionSource() , this);

            HaveSourcesNode hasMoneyForLicence = new HaveSourcesNode(actionBuyLicence, EA, () => { return GetPriceForMarketLicence(activeState.activeLicenceKind, activeState.activeSourceKind); }, map);
            DecisionBinaryNode hasSecondLicence = new DecisionBinaryNode(EA, hasMoneyForLicence, () => { return (activeState.activeLicenceKind = map.GetPlayerMe().GetMarketLicence(activeState.activeSourceKind)) == LicenceKind.SecondLicence; });
            DecisionBinaryNode everyTurnALotOfOneSource = new DecisionBinaryNode(hasSecondLicence, falseNode, () => { return ai.EveryTurnALotOfOneSource(48); });
            DecisionBinaryNode hasMarketWithFreeSlot = new DecisionBinaryNode(everyTurnALotOfOneSource, EA, ai.HasFreeSlotInMarket);

            return hasMarketWithFreeSlot;
        }

        public AIEasy GetAI() { return ai; }
        public void SetWasAction(bool action) { wasAction = action; }
        public bool GetWasAction() { return wasAction; }

        internal void SetActiveObject(SourceKind maxKind) { activeState.activeSourceKind = maxKind; }
        public void SetActiveObject(ITown town) { activeState.activeTown = town; }
        public ITown GetActiveTown() { return activeState.activeTown; }
        public void SetActiveObject(IRoad road) { activeState.activeRoad = road; }
        public IRoad GetActiveRoad() { return activeState.activeRoad; }

        public void SetActivePosInTown(byte pos) { activeState.activeTownPos = pos; }

        public void SolveAI()
        {
            do
            {
                ClearActiveObjects();
                actionSource.Clear();
                wasAction = false;
                root.Execute();
                if (!wasAction)
                {
                    TryMakeAction();
                }
            } while (wasAction);
        }

        public void ClearActiveObjects()
        {
            activeState = new ActiveState();
        }

        public void AddActionSource(ActionSource action)
        {
            action.SetSourceChange(map.CanChangeSourcesFor(map.GetPrice(action.GetPriceKind())));
            action.SetState(activeState);
            actionSource.Add(action);
        }

        private void TryMakeAction()
        {
            if (actionSource.Count > 0)
            {
                ActionSource bestAction = null;
                int bestPrice = 1000;

                foreach (ActionSource action in actionSource)
                {
                    int price = action.GetSourceChange();
                    if (price + 15 < bestPrice)
                    {
                        bestPrice = price;
                        bestAction = action;
                    }
                }

                {
                    activeState = bestAction.GetState();
                    map.ChangeSourcesFor(map.GetPrice(bestAction.GetPriceKind()));
                    bestAction.GetAction()();
                    SetWasAction(true);
                }
            }
        }

        public PriceKind GetPriceForMarketLicence(LicenceKind licenceKind, SourceKind sourceKind)
        {
            switch (licenceKind)
            {
                case LicenceKind.NoLicence :
                    switch (sourceKind)
                    {
                        case SourceKind.Corn: return PriceKind.MCorn1;
                        case SourceKind.Meat: return PriceKind.MMeat1;
                        case SourceKind.Stone: return PriceKind.MStone1;
                        case SourceKind.Wood: return PriceKind.MWood1;
                        case SourceKind.Ore: return PriceKind.MOre1;
                    }
                    break;

                case LicenceKind.FirstLicence:
                    switch (sourceKind)
                    {
                        case SourceKind.Corn: return PriceKind.MCorn2;
                        case SourceKind.Meat: return PriceKind.MMeat2;
                        case SourceKind.Stone: return PriceKind.MStone2;
                        case SourceKind.Wood: return PriceKind.MWood2;
                        case SourceKind.Ore: return PriceKind.MOre2;
                    }
                    break;
            }
            return PriceKind.MCorn1;
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
