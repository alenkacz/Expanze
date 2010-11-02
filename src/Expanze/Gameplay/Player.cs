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
            corn -= cost.corn;
            wood -= cost.wood;
            ore -= cost.ore;
            meat -= cost.meat;
            stone -= cost.stone;
        }

        public void addSources(SourceCost amount)
        {
            corn += amount.corn;
            wood += amount.wood;
            stone += amount.stone;
            meat += amount.meat;
            ore += amount.ore;
        }

        public void addSources(int wood, int stone, int corn, int meat, int ore)
        {
            this.corn += corn;
            this.wood += wood;
            this.ore += ore;
            this.meat += meat;
            this.stone += stone;
        }
    }
}
