﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;
using Expanze.Gameplay;

namespace Expanze
{
    public enum TransactionState {TransactionStart, TransactionMiddle, TransactionEnd };

    class Player : IPlayer
    {
        private String name;
        private Color color;
        private bool materialChanged;
        private int points;
        private bool active;            /// is he still playing? (ex. AI make exception and it cant continue in game

        SourceAll prevSource;       // Source before last source change (for example before paying for something, collecting resources
        SourceAll source;
        SourceAll transactionSource;
        SourceAll collectSourcesLastTurn; /// sources getted on the start of player's last turn
        SourceAll collectSourcesNormal;   /// collect sources without disasters and miracles

        int[] buildingCount;

        LicenceKind[] licenceMarket;
        UpgradeKind[] upgradeMonastery;

        List<IMonastery> monastery;
        List<IMarket> market;
        List<IFort> fort;

        IComponentAI componentAI;   // is null if player is NOT controled by computer but is controled by human

        private Statistic statistic;

        public Player(String name, Color color, IComponentAI componentAI)
        {
            points = 0;
            prevSource = new SourceAll(0);
            source = new SourceAll(0);
            transactionSource = new SourceAll(0);

            collectSourcesNormal = new SourceAll(0);

            buildingCount = new int[(int) Building.Count];
            for (int loop1 = 0; loop1 < (int)Building.Count; loop1++)
            {
                buildingCount[loop1] = 0;
            }

            licenceMarket = new LicenceKind[(int)SourceKind.Count];
            for (int loop1 = 0; loop1 < (int)SourceBuildingKind.Count; loop1++)
            {
                licenceMarket[loop1] = LicenceKind.NoLicence;
            }

            upgradeMonastery = new UpgradeKind[(int) SourceBuildingKind.Count];
            for (int loop1 = 0; loop1 < (int)SourceBuildingKind.Count; loop1++)
            {
                upgradeMonastery[loop1] = UpgradeKind.NoUpgrade;
            }

            this.color = color;
            this.name = name;
            this.componentAI = componentAI;

            materialChanged = false;
            active = true;

            monastery = new List<IMonastery>();
            market = new List<IMarket>();
            fort = new List<IFort>();

            statistic = new Statistic();
        }

        public int GetBuildingCount(Building building) { return buildingCount[(int)building]; }
        public IComponentAI GetComponentAI() { return componentAI; }
        public bool GetIsAI() { return componentAI != null; }
        public Color GetColor() { return color; }
        public bool GetActive() { return active; }
        public UpgradeKind GetMonasteryUpgrade(SourceBuildingKind kind) { return upgradeMonastery[(int)kind]; }
        public LicenceKind GetMarketLicence(SourceKind kind) { return licenceMarket[(int)kind]; }
        public Statistic GetStatistic() { return statistic; }

        public void AddBuilding(Building building)
        {
            buildingCount[(int)building]++;
            GameMaster.Inst().PlayerWantMedail(this, building);

            int turn = GameMaster.Inst().GetTurnNumber();
            switch (building)
            {
                case Building.Town: statistic.AddStat(Statistic.Kind.Towns, 1, turn); break;
                case Building.Road: statistic.AddStat(Statistic.Kind.Roads, 1, turn); break;
                case Building.Fort: statistic.AddStat(Statistic.Kind.Fort, 1, turn); break;
                case Building.Monastery: statistic.AddStat(Statistic.Kind.Monastery, 1, turn); break;
                case Building.Market: statistic.AddStat(Statistic.Kind.Market, 1, turn); break;
            }
        }

        public void AddMarket(IMarket m)
        {
            market.Add(m);
        }

        public List<IMarket> GetMarket()
        {
            return market;
        }

        public void AddMonastery(IMonastery m)
        {
            monastery.Add(m);
        }

        public List<IMonastery> GetMonastery() { return monastery; }

        public void AddFort(IFort f)
        {
            fort.Add(f);
        }

        public void AddPoints(int add) { 
            points += add;
            statistic.AddStat(Statistic.Kind.Points, add, GameMaster.Inst().GetTurnNumber());
            GameMaster.Inst().CheckWinner(this);
        }

        public int GetPoints() { return points; }

        public int GetConversionRate(SourceKind kind)
        {
            if (kind == SourceKind.Null)
                return -1;

            switch (licenceMarket[(int)kind])
            {
                case LicenceKind.NoLicence: return 4;
                case LicenceKind.FirstLicence: return 3;
                case LicenceKind.SecondLicence: return 2;
            }

            return -1;
        }

        public ISourceAll GetSource()
        {
            return source;
        }

        public String GetName()
        {
            return this.name;
        }

        public int GetCorn()
        {
            return source.corn;
        }

        public int GetWood()
        {
            return source.wood;
        }

        public int GetOre()
        {
            return source.ore;
        }

        public int GetMeat()
        {
            return source.meat;
        }

        public int GetStone()
        {
            return source.stone;
        }

        public void PayForSomething(ISourceAll cost)
        {
            SourceAll sourceCost = (SourceAll)cost;

            ChangeSources(-sourceCost.wood, -sourceCost.stone, -sourceCost.corn, -sourceCost.meat, -sourceCost.ore);

            source = source - sourceCost;
        }

        public void ClearCollectSources()
        {
            collectSourcesLastTurn = new SourceAll(0);
            collectSourcesNormal = new SourceAll(0);
        }

        public void AddCollectSources(SourceAll normal, SourceAll now)
        {
            collectSourcesNormal += normal;
            collectSourcesLastTurn += now;
        }

        public ISourceAll GetCollectSourcesNormal() { return collectSourcesNormal; }
        public SourceAll GetCollectSourcesLastTurn() { return collectSourcesLastTurn; }

        public void AddSources(SourceAll amount, TransactionState state)
        {
            switch (state)
            {
                case TransactionState.TransactionStart :
                    if (transactionSource.Equals(new SourceAll(0)))
                        transactionSource = amount;
                    else
                        transactionSource += amount;
                    break;
                case TransactionState.TransactionMiddle :
                    transactionSource = transactionSource + amount;
                    break;
                case TransactionState.TransactionEnd :
                    transactionSource = transactionSource + amount;
                    source = source + transactionSource;               
                    ChangeSources(transactionSource.wood, transactionSource.stone, transactionSource.corn, transactionSource.meat, transactionSource.ore);
                    transactionSource = new SourceAll(0);
                    GameMaster.Inst().CheckWinner(this);
                    break;
            }
        }
        
        /// <summary>
        /// Remembers state of material from previous round, active when active player is changed
        /// </summary>
        public void ChangeSources(int wood, int stone, int corn, int meat, int ore)
        {
            materialChanged = true;
            prevSource = new SourceAll(wood, stone, corn, meat, ore);
        }

        public bool HasMaterialChanged()
        {
            bool temp = materialChanged;
            materialChanged = false;
            return temp;
        }

        public void SetMaterialChange(SourceAll change)
        {
            materialChanged = true;
            prevSource = change;
        }
        public SourceAll GetMaterialChange()
        {
            return prevSource;
        }

        public bool HaveEnoughMaterialForConversion(SourceKind kind)
        {
            return source.Get(kind) >= GetConversionRate(kind);
        }

        public int GetMaterialNumber(SourceKind k)
        {
            return source.Get(k);
        }

        public void SetActive(bool active) 
        {
            if (this.active && !active)
                GameMaster.Inst().AddToPlayerCount(-1);

            this.active = active; 
            
        }

        public void SetSourceBuildingUpdate(UpgradeKind upgradeKind, int upgradeNumber)
        {
            statistic.AddStat(Statistic.Kind.Upgrades, 1, GameMaster.Inst().GetTurnNumber());
            upgradeMonastery[upgradeNumber] = upgradeKind;
        }

        public void BuyMarketLicence(LicenceKind licenceKind, int upgradeNumber)
        {
            statistic.AddStat(Statistic.Kind.Licences, 1, GameMaster.Inst().GetTurnNumber());
            licenceMarket[upgradeNumber] = licenceKind;
        }

        public void AddFortAction()
        {
            statistic.AddStat(Statistic.Kind.Actions, 1, GameMaster.Inst().GetTurnNumber());
        }
    }
}
