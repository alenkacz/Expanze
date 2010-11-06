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

        public SourceCost(int amount)
        {
            corn = amount;
            wood = amount;
            stone = amount;
            meat = amount;
            ore = amount;
        }

        public SourceCost(int wood, int stone, int corn, int meat, int ore)
        {
            this.corn = corn;
            this.wood = wood;
            this.stone = stone;
            this.meat = meat;
            this.ore = ore;
        }

        public static SourceCost operator +(SourceCost a, SourceCost b)
        {
            return new SourceCost(a.wood + b.wood, a.stone + b.stone, a.corn + b.corn, a.meat + b.meat, a.ore + b.ore);
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
