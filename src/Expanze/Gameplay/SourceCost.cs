using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class SourceAll : ISourceAll
    {
        public int corn = 0;
        public int wood = 0;
        public int stone = 0;
        public int meat = 0;
        public int ore = 0;

        public SourceAll(int amount)
        {
            corn = amount;
            wood = amount;
            stone = amount;
            meat = amount;;
            ore = amount;
        }

        public SourceAll(int wood, int stone, int corn, int meat, int ore)
        {
            this.corn = corn;
            this.wood = wood;
            this.stone = stone;
            this.meat = meat;
            this.ore = ore;
        }

        public int GetWood() { return wood; }
        public int GetStone() { return stone; }
        public int GetCorn() { return corn; }
        public int GetOre() { return ore; }
        public int GetMeat() { return meat; }

        public static SourceAll operator +(SourceAll a, SourceAll b)
        {
            return new SourceAll(a.wood + b.wood, a.stone + b.stone, a.corn + b.corn, a.meat + b.meat, a.ore + b.ore);
        }

        public static SourceAll operator -(SourceAll a, SourceAll b)
        {
            return new SourceAll(a.wood - b.wood, a.stone - b.stone, a.corn - b.corn, a.meat - b.meat, a.ore - b.ore);
        }

        public override bool Equals(object obj)
        {
            SourceAll b = (SourceAll)obj;
            return b.corn == corn &&
                   b.meat == meat &&
                   b.stone == stone &&
                   b.wood == wood &&
                   b.ore == ore;
        }

        public int this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0: return corn;
                    case 1: return meat;
                    case 2: return stone;
                    case 3: return wood;
                    case 4: return ore;
                    default: return -1;
                }
            }
        }

        public Boolean HasPlayerSources(Player player)
        {
            return player.getCorn() >= corn &&
                   player.getMeat() >= meat &&
                   player.getOre() >= ore &&
                   player.getStone() >= stone &&
                   player.getWood() >= wood;
        }

        public SourceKind IntToKind(int index)
        {
            switch (index)
            {
                case 0: return SourceKind.Corn;
                case 1: return SourceKind.Meat;
                case 2: return SourceKind.Stone;
                case 3: return SourceKind.Wood;
                case 4: return SourceKind.Ore;
                default: return SourceKind.Count; // shouldnt happened;
            }
        }

        public int Get(SourceKind sourceKind)
        {
            switch (sourceKind)
            {
                case SourceKind.Corn: return corn;
                case SourceKind.Meat: return meat;
                case SourceKind.Ore: return ore;
                case SourceKind.Stone: return stone;
                case SourceKind.Wood: return wood;
                default: return 0;
            }
        }
    }
}
