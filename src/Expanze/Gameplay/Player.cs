using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze
{
    public enum TransactionState {TransactionStart, TransactionMiddle, TransactionEnd };
    class Player
    {
        private String name;
        private Color color;
        private bool materialChanged;
        private int points;

        SourceAll prevSource;       // Source before last source change (for example before paying for something, collecting resources
        SourceAll source;
        SourceAll transactionSource;
        int conversionRateCorn;
        int conversionRateStone;
        int conversionRateOre;
        int conversionRateMeat;
        int conversionRateWood;

        IComponentAI componentAI;   // is null if player is NOT controled by computer but is controled by human

        public Player(String name, Color color, IComponentAI componentAI)
        {
            points = 0;
            prevSource = new SourceAll(0);
            source = new SourceAll(0);
            transactionSource = new SourceAll(0);

            this.conversionRateCorn = Settings.conversionRateCorn;
            conversionRateStone = Settings.conversionRateStone;
            conversionRateOre = Settings.conversionRateOre;
            conversionRateMeat = Settings.conversionRateMeat;
            conversionRateWood = Settings.conversionRateWood;

            this.color = color;
            this.name = name;
            this.componentAI = componentAI;

            materialChanged = false;
        }

        public IComponentAI getComponentAI() { return componentAI; }
        public bool getIsAI() { return componentAI != null; }
        public Color getColor() { return color; }

        public void addPoints(int add) { 
            points += add;
            GameMaster.getInstance().checkWinner(this);
        }

        public int getPoints() { return points; }

        public int getConversionRate(HexaKind h)
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

        public SourceAll getSource()
        {
            return source;
        }

        public String getName()
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

        public void payForSomething(SourceAll cost)
        {
            changeSources(-cost.wood, -cost.stone, -cost.corn, -cost.meat, -cost.ore);

            source = source - cost;
        }

        public void addSources(SourceAll amount, TransactionState state)
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
                    changeSources(transactionSource.wood, transactionSource.stone, transactionSource.corn, transactionSource.meat, transactionSource.ore);
                    transactionSource = new SourceAll(0);
                    GameMaster.getInstance().checkWinner(this);
                    break;
            }
        }
        
        /// <summary>
        /// Remembers state of material from previous round, active when active player is changed
        /// </summary>
        public void changeSources(int wood, int stone, int corn, int meat, int ore)
        {
            materialChanged = true;
            prevSource = new SourceAll(wood, stone, corn, meat, ore);
        }

        public bool hasMaterialChanged()
        {
            bool temp = materialChanged;
            materialChanged = false;
            return temp;
        }

        public SourceAll getMaterialChange()
        {
            return prevSource;
        }

        public bool haveEnoughMaterial(HexaKind k)
        {
            switch (k)
            {
                case HexaKind.Cornfield:
                    return getCorn() > getConversionRate(k);
                case HexaKind.Forest:
                    return getWood() > getConversionRate(k);
                case HexaKind.Mountains:
                    return getOre() > getConversionRate(k);
                case HexaKind.Pasture:
                    return getMeat() > getConversionRate(k);
                case HexaKind.Stone:
                    return getStone() > getConversionRate(k);
            }

            return false;
        }
    }
}
