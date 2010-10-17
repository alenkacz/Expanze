using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    class Player
    {
        private int score;
        private String name;

        public Player(int score, String name)
        {
            this.score = score;
            this.name = name;
        }

        public String getName()
        {
            return this.name;
        }
    }
}
