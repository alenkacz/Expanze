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
        private int stone = 0;
        private int sheep = 0;
        private int brick = 0;

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

        public int getStone()
        {
            return this.stone;
        }

        public int getSheep()
        {
            return this.sheep;
        }

        public int getBrick()
        {
            return this.brick;
        }
    }
}
