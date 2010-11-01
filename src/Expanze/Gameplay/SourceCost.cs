using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    class SourceCost
    {
        public int corn = 0;
        public int wood = 0;
        public int stone = 0;
        public int meat = 0;
        public int ore = 0;

        public SourceCost()
        {
            corn = 0;
            wood = 0;
            stone = 0;
            meat = 0;
            ore = 0;
        }

        public SourceCost(int wood, int stone, int corn, int meat, int ore)
        {
            this.corn = corn;
            this.wood = wood;
            this.stone = stone;
            this.meat = meat;
            this.ore = ore;
        }

        public Boolean HasPlayerSources(Player player)
        {
            return player.getCorn() >= corn &&
                   player.getMeat() >= meat &&
                   player.getOre() >= ore &&
                   player.getStone() >= stone &&
                   player.getWood() >= wood;
        }
    }
}
