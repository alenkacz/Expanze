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

            DecisionBinaryNode haveFort2 = new DecisionBinaryNode(MakeActionStealSources(MakeActionShowParade()), EA, () => { return map.GetPlayerMe().GetBuildingCount(Building.Fort) > 0; });


            ITreeNode upgradeTree = MakeInventUpgradeTree(haveFort2);
            ITreeNode licenceTree = MakeBuyLicenceTree(upgradeTree);
            CanHaveSourcesNode canHaveSourceForRoad = new CanHaveSourcesNode(MakeBuildRoadTree(licenceTree), licenceTree, PriceKind.BRoad, map);

            List<ITreeNode> specialBuildingBuildList;
            specialBuildingBuildList = new List<ITreeNode>();
            specialBuildingBuildList.Add(MakeBuildMarketTree());
            specialBuildingBuildList.Add(MakeBuildMonasteryTree());
            specialBuildingBuildList.Add(MakeBuildFortTree());

            StochasticNodeMultiple specialBuildingBuild = new StochasticNodeMultiple(specialBuildingBuildList, EA, this);
            DecisionBinaryNode canBuildSpecialBuilding = new DecisionBinaryNode(specialBuildingBuild, EA, () => { return activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind() != HexaKind.Mountains; });
            CanHaveSourcesNode canHaveSourceForSourceBuilding = new CanHaveSourcesNode(MakeBuildSourceBuildingTree(canBuildSpecialBuilding), canBuildSpecialBuilding, () => { return GetPriceForSourceBuilding(activeState.activeTown, activeState.activeTownPos); }, map);
            DecisionBinaryNode isThatHexaDesert = new DecisionBinaryNode(canBuildSpecialBuilding, canHaveSourceForSourceBuilding, () => { return activeState.activeTown.GetIHexa(activeState.activeTownPos).GetKind() == HexaKind.Desert; });

            ForEachFreeHexInTownNode forEachFreeHexaInTown = new ForEachFreeHexInTownNode(isThatHexaDesert, canHaveSourceForRoad, this);
            DecisionBinaryNode hasFreeHexaInTown = new DecisionBinaryNode(forEachFreeHexaInTown, canHaveSourceForRoad, ai.IsFreeHexaInTown);
            CanHaveSourcesNode canHaveSourceForTown = new CanHaveSourcesNode(MakeBuildTownTree(hasFreeHexaInTown), hasFreeHexaInTown, PriceKind.BTown, map);

            DecisionBinaryNode haveFort1 = new DecisionBinaryNode(MakeActionStealSources(canHaveSourceForTown), canHaveSourceForTown, () => { return map.GetPlayerMe().GetBuildingCount(Building.Fort) > 0; });

            root = haveFort1;
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

            DecisionBinaryNode hasFort = new DecisionBinaryNode(haveSourcesForMarket, EA, () => map.GetPlayerMe().GetBuildingCount(Building.Fort) > 0);
            DecisionBinaryNode hasMarket = new DecisionBinaryNode(hasFort, haveSourcesForMarket, () => map.GetPlayerMe().GetBuildingCount(Building.Market) > 0);
            DecisionBinaryNode hasMarketWithFreeSlot = new DecisionBinaryNode(EA, hasMarket, ai.HasFreeSlotInMarket);
            DecisionBinaryNode everyTurnALotOfOneSource = new DecisionBinaryNode(hasMarketWithFreeSlot, EA, () => { return ai.EveryTurnALotOfOneSource(54, true); });  
            CanHaveSourcesNode canHaveSourcesForMarket = new CanHaveSourcesNode(everyTurnALotOfOneSource, EA, PriceKind.BMarket, map);

            DecisionBinaryNode hasSourcesLess16 = new DecisionBinaryNode(canHaveSourcesForMarket, EA, () => activeState.activeTown.GetIHexa(activeState.activeTownPos).GetStartSource() <= 16);

            return hasSourcesLess16;
        }

        private ITreeNode MakeBuyLicenceTree(ITreeNode falseNode)
        {
            DelAction buyLicence = () => ai.BuyLicence(activeState.activeSourceKind);
            ActionNode actionBuyLicence = new ActionNode(buyLicence, this);
            GetPrice getLicencePrice = () => { return GetPriceForMarketLicence(activeState.activeLicenceKind, activeState.activeSourceKind);  };
            ChangeSourcesActionNode addActionBuyLicence = new ChangeSourcesActionNode(new ActionSource(buyLicence, getLicencePrice), this);

            HaveSourcesNode hasMoneyForLicence = new HaveSourcesNode(actionBuyLicence, EA, getLicencePrice, map);
            CanHaveSourcesNode canHaveSourcesForLicence = new CanHaveSourcesNode(hasMoneyForLicence, falseNode, getLicencePrice, map);
            DecisionBinaryNode hasSecondLicence = new DecisionBinaryNode(falseNode, canHaveSourcesForLicence, () => { return (activeState.activeLicenceKind = map.GetPlayerMe().GetMarketLicence(activeState.activeSourceKind)) == LicenceKind.SecondLicence; });
            DecisionBinaryNode everyTurnALotOfOneSource = new DecisionBinaryNode(hasSecondLicence, falseNode, () => { return ai.EveryTurnALotOfOneSource(54, true); });
            DecisionBinaryNode hasMarketWithFreeSlot = new DecisionBinaryNode(everyTurnALotOfOneSource, falseNode, ai.HasFreeSlotInMarket);

            return hasMarketWithFreeSlot;
        }

        private ITreeNode MakeBuildMonasteryTree()
        {
            ActionNode actionBuildMonastery = new ActionNode(() => ai.BuildMonastery(activeState.activeTown, activeState.activeTownPos), this);
            ChangeSourcesActionNode addActionMonastery = new ChangeSourcesActionNode(new ActionSource(() => ai.BuildMonastery(activeState.activeTown, activeState.activeTownPos), PriceKind.BMonastery), this);

            HaveSourcesNode haveSourcesForMonastery = new HaveSourcesNode(actionBuildMonastery, addActionMonastery, PriceKind.BMonastery, map);

            DecisionBinaryNode hasFort = new DecisionBinaryNode(haveSourcesForMonastery, EA, () => map.GetPlayerMe().GetBuildingCount(Building.Fort) > 0);
            DecisionBinaryNode hasMonastery = new DecisionBinaryNode(hasFort, haveSourcesForMonastery, () => map.GetPlayerMe().GetBuildingCount(Building.Monastery) > 0);

            DecisionBinaryNode hasMonasteryWithFreeSlot = new DecisionBinaryNode(EA, hasMonastery, ai.HasFreeSlotInMonastery);
            DecisionBinaryNode everyTurnALotOfOneSource = new DecisionBinaryNode(hasMonasteryWithFreeSlot, EA, () => { return ai.EveryTurnALotOfOneSource(60, false); });
            CanHaveSourcesNode canHaveSourcesForMonastery = new CanHaveSourcesNode(everyTurnALotOfOneSource, EA, PriceKind.BMonastery, map);

            DecisionBinaryNode hasSourcesLess16 = new DecisionBinaryNode(canHaveSourcesForMonastery, EA, () => activeState.activeTown.GetIHexa(activeState.activeTownPos).GetStartSource() <= 16);


            return hasSourcesLess16;
        }

        private ITreeNode MakeInventUpgradeTree(ITreeNode falseNode)
        {
            DelAction inventUpgrade = () => ai.InventUpgrade(activeState.activeSourceBuildingKind);
            ActionNode actionInventUpgrade = new ActionNode(inventUpgrade, this);
            GetPrice getUpgradePrice = () => { return GetPriceForMonasteryUpgrade(activeState.activeUpgradeKind, activeState.activeSourceBuildingKind);};
            ChangeSourcesActionNode addActionInventUpgrade = new ChangeSourcesActionNode(new ActionSource(inventUpgrade, getUpgradePrice), this);

            HaveSourcesNode hasMoneyForUpgrade = new HaveSourcesNode(actionInventUpgrade, addActionInventUpgrade, getUpgradePrice, map);
            CanHaveSourcesNode canHaveMoneyForUpgrade = new CanHaveSourcesNode(hasMoneyForUpgrade, falseNode, getUpgradePrice, map);
            DecisionBinaryNode hasSecondUpgrade = new DecisionBinaryNode(falseNode, canHaveMoneyForUpgrade, () => { return (activeState.activeUpgradeKind = map.GetPlayerMe().GetMonasteryUpgrade(activeState.activeSourceBuildingKind)) == UpgradeKind.SecondUpgrade; });
            DecisionBinaryNode everyTurnALotOfOneSource = new DecisionBinaryNode(hasSecondUpgrade, falseNode, () => { return ai.EveryTurnALotOfOneSource(60, false); });
            DecisionBinaryNode hasMonasteryWithFreeSlot = new DecisionBinaryNode(everyTurnALotOfOneSource, falseNode, ai.HasFreeSlotInMonastery);

            return hasMonasteryWithFreeSlot;
        }

        private ITreeNode MakeBuildFortTree()
        {
            DelAction buildFort = () => ai.BuildFort(activeState.activeTown, activeState.activeTownPos);
            ActionNode actionBuildFort = new ActionNode(buildFort, this);
            ChangeSourcesActionNode addActionBuildFort = new ChangeSourcesActionNode(new ActionSource(buildFort, PriceKind.BFort), this);

            HaveSourcesNode haveSourcesForFort = new HaveSourcesNode(actionBuildFort, addActionBuildFort, PriceKind.BFort, map);
            DecisionBinaryNode existPlayerWithFitnessMore = new DecisionBinaryNode(haveSourcesForFort, EA, () => { return ai.ExistPlayerWithFitnessMore(0.35f); });
            DecisionBinaryNode existFreePlaceForRoad = new DecisionBinaryNode(existPlayerWithFitnessMore, haveSourcesForFort, ai.ExistFreePlaceForRoad);
            DecisionBinaryNode hasFortAlready = new DecisionBinaryNode(EA, existFreePlaceForRoad, () => { return map.GetPlayerMe().GetBuildingCount(Building.Fort) > 0; });
            CanHaveSourcesNode canHaveSourcesForFort = new CanHaveSourcesNode(hasFortAlready, EA, PriceKind.BFort, map);

            DecisionBinaryNode hasSourcesLess16 = new DecisionBinaryNode(canHaveSourcesForFort, EA, () => activeState.activeTown.GetIHexa(activeState.activeTownPos).GetStartSource() <= 16);

            return hasSourcesLess16;
        }

        
        private ITreeNode MakeActionShowParade()
        {
            DelAction showParade = () => { return ai.ActionShowParade(); };
            ActionNode actionShowParade = new ActionNode(showParade, this);
            ChangeSourcesActionNode addActionShowParade = new ChangeSourcesActionNode(new ActionSource(showParade, PriceKind.AParade), this);

            HaveSourcesNode haveSourcesForParade = new HaveSourcesNode(actionShowParade, addActionShowParade, PriceKind.AParade, map);
            DecisionBinaryNode existFreePlaceForRoad = new DecisionBinaryNode(EA, haveSourcesForParade, ai.ExistFreePlaceForRoad);
            CanHaveSourcesNode canHaveSourcesForParade = new CanHaveSourcesNode(existFreePlaceForRoad, EA, PriceKind.AParade, map);

            return canHaveSourcesForParade;
        }

        private ITreeNode MakeActionStealSources(ITreeNode falseNode)
        {
            DelAction stealSources = () => { return ai.StealSources(activeState.activePlayer); };
            ActionNode actionStealSources = new ActionNode(stealSources, this);
            ChangeSourcesActionNode addActionStealSources = new ChangeSourcesActionNode(new ActionSource(stealSources, PriceKind.AStealSources), this);

            HaveSourcesNode haveSourcesForStealSources = new HaveSourcesNode(actionStealSources, addActionStealSources, PriceKind.AStealSources, map);
            DecisionBinaryNode enoughFitness = new DecisionBinaryNode(haveSourcesForStealSources, falseNode, () => { return Fitness.GetFitness(activeState.activePlayer) > 0.35; });
            ForEachPlayerNode forEachPlayerNode = new ForEachPlayerNode(enoughFitness, falseNode, this);
            CanHaveSourcesNode canHaveSourcesForStealSources = new CanHaveSourcesNode(forEachPlayerNode, falseNode, PriceKind.AStealSources, map);
            return canHaveSourcesForStealSources;
        }
        
        public AIEasy GetAI() { return ai; }
        public IMapController GetMapController() { return map; }
        public void SetWasAction(bool action) { wasAction = action; }
        public bool GetWasAction() { return wasAction; }

        internal void SetActiveObject(SourceBuildingKind kind) { activeState.activeSourceBuildingKind = kind; }
        internal void SetActiveObject(SourceKind kind) { activeState.activeSourceKind = kind; }
        public void SetActiveObject(ITown town) { activeState.activeTown = town; }
        public ITown GetActiveTown() { return activeState.activeTown; }
        public void SetActiveObject(IRoad road) { activeState.activeRoad = road; }
        public IRoad GetActiveRoad() { return activeState.activeRoad; }

        public void SetActivePosInTown(byte pos) { activeState.activeTownPos = pos; }
        public void SetActiveObject(IPlayer player) { activeState.activePlayer = player; }

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
            ActionSource actionS = new ActionSource(action.GetAction(), action.GetPriceKind());

            actionS.SetSourceChange(map.CanChangeSourcesFor(map.GetPrice(actionS.GetPriceKind())));
            actionS.SetState(activeState);
            if (actionS.GetSourceChange() >= 0)
                actionSource.Add(actionS);
            else
            {
                throw new Exception("You cant have sources for " + actionS.ToString());
            }
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
                    //if (map.CanChangeSourcesFor(map.GetPrice(bestAction.GetPriceKind())) >= 0)
                    {
                        PriceKind kind = bestAction.GetPriceKind();
                        map.ChangeSourcesFor(map.GetPrice(kind));
                        bestAction.GetAction()();
                        SetWasAction(true);
                    }
                }
            }
        }

        private PriceKind GetPriceForMarketLicence(LicenceKind licenceKind, SourceKind sourceKind)
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

        private PriceKind GetPriceForMonasteryUpgrade(UpgradeKind upgradeKind, SourceBuildingKind buildingKind)
        {
            switch (upgradeKind)
            {
                case UpgradeKind.NoUpgrade :
                    switch (buildingKind)
                    {
                        case SourceBuildingKind.Mill: return PriceKind.UMill1;
                        case SourceBuildingKind.Stepherd: return PriceKind.UStepherd1;
                        case SourceBuildingKind.Quarry: return PriceKind.UQuarry1;
                        case SourceBuildingKind.Saw: return PriceKind.USaw1;
                        case SourceBuildingKind.Mine: return PriceKind.UMine1;
                    }
                    break;
                case UpgradeKind.FirstUpgrade :
                    switch (buildingKind)
                    {
                        case SourceBuildingKind.Mill: return PriceKind.UMill2;
                        case SourceBuildingKind.Stepherd: return PriceKind.UStepherd2;
                        case SourceBuildingKind.Quarry: return PriceKind.UQuarry2;
                        case SourceBuildingKind.Saw: return PriceKind.USaw2;
                        case SourceBuildingKind.Mine: return PriceKind.UMine2;
                    }
                    break;
            }
            return PriceKind.UMill1;
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
