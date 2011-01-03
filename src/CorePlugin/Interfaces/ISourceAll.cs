using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    /// <summary>
    /// Enum for all sources kind in game. Corn, meat, stone, wood, ore.
    /// </summary>
    public enum SourceKind { Corn, Meat, Stone, Wood, Ore, Count, Null };

    /// <summary>
    /// All errors which can be when you change sources in market.
    /// </summary>
    public enum ChangingSourcesError { OK, NotEnoughFromSource};

    public interface ISourceAll
    {
        int GetWood();
        int GetOre();
        int GetMeat();
        int GetStone();
        int GetCorn();

        /// <summary>
        /// Transfer source kind to int.
        /// Corn 0
        /// Meat 1
        /// Stone 2
        /// Wood 3
        /// Ore 4
        /// </summary>
        /// <param name="sourceKind">Source kind to be transfered</param>
        /// <returns>Number between 0 - 4</returns>
        int Get(SourceKind sourceKind);

        /// <summary>
        /// Opposite of int Get(SourceKind sourceKind);
        /// </summary>
        /// <param name="index">Number to be transfered</param>
        /// <returns>Source kind according to index number</returns>
        SourceKind IntToKind(int index);

        int this[int index]
        {
            get;
        }
    }
}
