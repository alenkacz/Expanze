﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze
{
    public enum TransactionState {TransactionStart, TransactionMiddle, TransactionEnd };
    public enum Building { Town, Road, Market, Monastery, Fort, Mill, Stepherd, Quarry, Saw, Mine, Count }

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

        int[] buildingCount;

        int conversionRateCorn;
        int conversionRateStone;
        int conversionRateOre;
        int conversionRateMeat;
        int conversionRateWood;
        UpgradeKind[] upgradeSourceBuilding;

        IComponentAI componentAI;   // is null if player is NOT controled by computer but is controled by human

        public Player(String name, Color color, IComponentAI componentAI)
        {
            points = 0;
            prevSource = new SourceAll(0);
            source = new SourceAll(0);
            transactionSource = new SourceAll(0);

            buildingCount = new int[(int) Building.Count];
            for (int loop1 = 0; loop1 < (int)Building.Count; loop1++)
            {
                buildingCount[loop1] = 0;
            }

            conversionRateCorn = Settings.conversionRateCorn;
            conversionRateStone = Settings.conversionRateStone;
            conversionRateOre = Settings.conversionRateOre;
            conversionRateMeat = Settings.conversionRateMeat;
            conversionRateWood = Settings.conversionRateWood;

            upgradeSourceBuilding = new UpgradeKind[(int) SourceBuildingKind.Count];
            for (int loop1 = 0; loop1 < (int)SourceBuildingKind.Count; loop1++)
            {
                upgradeSourceBuilding[loop1] = UpgradeKind.NoUpgrade;
            }

            this.color = color;
            this.name = name;
            this.componentAI = componentAI;

            materialChanged = false;
            active = true;
        }

        public int GetBuildingCount(Building building) { return buildingCount[(int)building]; }
        public IComponentAI GetComponentAI() { return componentAI; }
        public bool GetIsAI() { return componentAI != null; }
        public Color GetColor() { return color; }
        public bool GetActive() { return active; }
        public UpgradeKind GetSourceBuildingUpgrade(SourceBuildingKind kind) { return upgradeSourceBuilding[(int)kind]; }

        public void AddBuilding(Building building)
        {
            buildingCount[(int)building]++;
            GameMaster.Inst().PlayerWantMedail(this, building);
        }

        public void AddPoints(int add) { 
            points += add;
            GameMaster.Inst().CheckWinner(this);
        }

        public int GetPoints() { return points; }

        public int GetConversionRate(HexaKind h)
        {
            switch (h)
            {
                case HexaKind.Cornfield:
                    return conversionRateCorn;
                case HexaKind.Forest:
                    return conversionRateWood;
                case HexaKind.Mountains:
                    return conversionRateOre;
                case HexaKind.Pasture:
                    return conversionRateMeat;
                case HexaKind.Stone:
                    return conversionRateStone;
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

        public int getCorn()
        {
            return source.corn;
        }

        public int getWood()
        {
            return source.wood;
        }

        public int getOre()
        {
            return source.ore;
        }

        public int getMeat()
        {
            return source.meat;
        }

        public int getStone()
        {
            return source.stone;
        }

        public void PayForSomething(SourceAll cost)
        {
            ChangeSources(-cost.wood, -cost.stone, -cost.corn, -cost.meat, -cost.ore);

            source = source - cost;
        }

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

        public SourceAll GetMaterialChange()
        {
            return prevSource;
        }

        public bool HaveEnoughMaterial(HexaKind k)
        {
            switch (k)
            {
                case HexaKind.Cornfield:
                    return getCorn() > GetConversionRate(k);
                case HexaKind.Forest:
                    return getWood() > GetConversionRate(k);
                case HexaKind.Mountains:
                    return getOre() > GetConversionRate(k);
                case HexaKind.Pasture:
                    return getMeat() > GetConversionRate(k);
                case HexaKind.Stone:
                    return getStone() > GetConversionRate(k);
            }

            return false;
        }

        public int GetMaterialNumber(HexaKind k)
        {
            switch (k)
            {
                case HexaKind.Cornfield:
                    return getCorn();
                case HexaKind.Forest:
                    return getWood();
                case HexaKind.Mountains:
                    return getOre();
                case HexaKind.Pasture:
                    return getMeat();
                case HexaKind.Stone:
                    return getStone();
            }

            return 0;
        }

        public void SetActive(bool active) 
        {
            if (this.active && !active)
                GameMaster.Inst().AddToPlayerCount(-1);

            this.active = active; 
            
        }

        public void SetSourceBuildingUpdate(UpgradeKind upgradeKind, int upgradeNumber)
        {
            upgradeSourceBuilding[upgradeNumber] = upgradeKind;
        }

        public void SetMarketRate(UpgradeKind upgradeKind, int upgradeNumber)
        {
            switch (upgradeKind)
            {
                case UpgradeKind.FirstUpgrade :
                    switch (upgradeNumber)
                    {
                        case 0: conversionRateCorn = 3; break;
                        case 1: conversionRateMeat = 3; break;
                        case 2: conversionRateStone = 3; break;
                        case 3: conversionRateWood = 3; break;
                        case 4: conversionRateOre = 3; break;
                    }
                    break;
                case UpgradeKind.SecondUpgrade :
                    switch (upgradeNumber)
                    {
                        case 0: conversionRateCorn = 2; break;
                        case 1: conversionRateMeat = 2; break;
                        case 2: conversionRateStone = 2; break;
                        case 3: conversionRateWood = 2; break;
                        case 4: conversionRateOre = 2; break;
                    }
                    break;
            }
        }
    }
}
