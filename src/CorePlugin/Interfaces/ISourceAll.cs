using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum SourceKind { Corn, Meat, Stone, Wood, Ore, Count };
    public enum ChangingSourcesError { OK, NotEnoughFromSource};
    public interface ISourceAll
    {
        int getWood();
        int getOre();
        int getMeat();
        int getStone();
        int getCorn();
        int Get(SourceKind sourceKind);
        SourceKind IntToKind(int index);
        int this[int index]
        {
            get;
        }
    }
}
