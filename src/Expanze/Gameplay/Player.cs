using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze
{
    class Player
    {
        private int money;
        private String name;
        private Color color;
        private bool materialChangeDisplayed = true;

        private int prev_corn = 0;
        private int prev_wood = 0;
        private int prev_ore = 0;
        private int prev_meat = 0;
        private int prev_stone = 0;

        private int corn = 0;
        private int wood = 0;
        private int ore = 0;
        private int meat = 0;
        private int stone = 0;

        public Player(int score, String name, Color color)
        {
            this.color = color;
            this.money = score;
            this.name = name;
        }

        public Color getColor() { return color; }

        public String getName()
        {
            return this.name;
        }

        public int getCorn()
        {
            return this.corn;
        }

        public int getWood()
        {
            return this.wood;
        }

        public int getOre()
        {
            return this.ore;
        }

        public int getMeat()
        {
            return this.meat;
        }

        public int getStone()
        {
            return this.stone;
        }

        public void payForSomething(SourceCost cost)
        {
            changeSources(-cost.wood, -cost.stone, -cost.corn, -cost.meat, -cost.ore);

            corn -= cost.corn;
            wood -= cost.wood;
            ore -= cost.ore;
            meat -= cost.meat;
            stone -= cost.stone;
        }

        public void addSources(SourceCost amount)
        {
            changeSources(amount.wood, amount.stone, amount.corn, amount.meat, amount.ore);

            corn += amount.corn;
            wood += amount.wood;
            stone += amount.stone;
            meat += amount.meat;
            ore += amount.ore;
        }

        public void addSources(int wood, int stone, int corn, int meat, int ore)
        {
            changeSources(wood,stone,corn,meat,ore);

            this.corn += corn;
            this.wood += wood;
            this.ore += ore;
            this.meat += meat;
            this.stone += stone;
        }
        
        /// <summary>
        /// Remembers state of material from previous round, active when active player is changed
        /// </summary>
        public void changeSources(int wood, int stone, int corn, int meat, int ore)
        {
            if (materialChangeDisplayed)
            {
                prev_corn = corn;
                prev_wood = wood;
                prev_stone = stone;
                prev_meat = meat;
                prev_ore = ore;

                materialChangeDisplayed = false;
            }
        }

        public bool hasMaterialChanged()
        {
            return !materialChangeDisplayed;
        }

        public SourceCost getMaterialChange()
        {
            materialChangeDisplayed = true;
            return new SourceCost(prev_wood,prev_stone,prev_corn,prev_meat,prev_ore);
        }
    }
}
