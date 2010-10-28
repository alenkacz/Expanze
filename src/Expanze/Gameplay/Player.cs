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

        public void addSources(int corn, int wood, int stone, int sheep, int brick)
        {
            this.corn += corn;
            this.wood += wood;
            this.ore += stone;
            this.meat += sheep;
            this.stone += brick;
        }
    }
}
