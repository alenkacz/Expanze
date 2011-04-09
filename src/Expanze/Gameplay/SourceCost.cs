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

        public SourceAll(SourceAll b)
        {
            corn = b.corn;
            wood = b.wood;
            stone = b.stone;
            meat = b.meat;
            ore = b.ore;
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

        public SourceKind[] Order()
        {
            SourceKind[] kind = new SourceKind[(int) SourceKind.Count];
            kind[0] = SourceKind.Corn;
            kind[1] = SourceKind.Meat;
            kind[2] = SourceKind.Stone;
            kind[3] = SourceKind.Wood;
            kind[4] = SourceKind.Ore;

            int[] number = this;

            int min;
            int tempInt;
            SourceKind tempKind;

            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                min = loop1;
                for (int loop2 = loop1 + 1; loop2 < 5; loop2++)
                {
                    if (number[loop2] < number[min])
                        min = loop2;
                }

                tempInt = number[loop1];
                number[loop1] = number[min];
                number[min] = tempInt;

                tempKind = kind[loop1];
                kind[loop1] = kind[min];
                kind[min] = tempKind;
            }

            return kind;
        }

        public static SourceAll operator +(SourceAll a, SourceAll b)
        {
            return new SourceAll(a.wood + b.wood, a.stone + b.stone, a.corn + b.corn, a.meat + b.meat, a.ore + b.ore);
        }

        public static SourceAll operator -(SourceAll a)
        {
            return new SourceAll(- a.wood, - a.stone, - a.corn, - a.meat, - a.ore);
        }

        public static SourceAll operator -(SourceAll a, SourceAll b)
        {
            return new SourceAll(a.wood - b.wood, a.stone - b.stone, a.corn - b.corn, a.meat - b.meat, a.ore - b.ore);
        }

        public static SourceAll operator /(SourceAll s, int a)
        {
            return new SourceAll(s.GetWood() / a, s.GetStone() / a, s.GetCorn() / a, s.GetMeat() / a, s.GetOre() / a);
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

        public override int GetHashCode()
        {
            return corn + meat << 6 + stone << 12 + wood << 18 + ore << 24;
        }

        public static implicit operator int[](SourceAll a)
        {
            int[] b = new int[5];
            for (int loop1 = 0; loop1 < 5; loop1++)
                b[loop1] = a[loop1];
            return b;
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

            set
            {
                switch (index)
                {
                    case 0: corn = value; break;
                    case 1: meat = value; break;
                    case 2: stone = value; break;
                    case 3: wood = value; break;
                    case 4: ore = value; break;
                }
            }
        }

        public Boolean HasPlayerSources(IPlayer player)
        {
            ISourceAll sourcePlayer = player.GetSource();

            return sourcePlayer.GetCorn() >= corn &&
                   sourcePlayer.GetMeat() >= meat &&
                   sourcePlayer.GetOre() >= ore &&
                   sourcePlayer.GetStone() >= stone &&
                   sourcePlayer.GetWood() >= wood;
        }

        public int KindToInt(SourceKind kind)
        {
            switch (kind)
            {
                case SourceKind.Corn: return 0;
                case SourceKind.Meat: return 1;
                case SourceKind.Stone: return 2;
                case SourceKind.Wood: return 3;
                case SourceKind.Ore: return 4;
            }
            return -1;
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
