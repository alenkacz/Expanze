using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    class Player
    {
        private int money;
        private String name;

        private int corn = 0;
        private int wood = 0;
        private int ore = 0;
        private int meat = 0;
        private int stone = 0;

        public Player(int score, String name)
        {
            this.money = score;
            this.name = name;
        }

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
