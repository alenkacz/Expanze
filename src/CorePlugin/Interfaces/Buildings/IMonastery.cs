using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum MonasteryError { 
        /// <summary>
        /// There is no problem with inventing new upgrade.
        /// </summary>
        OK, 
        
        /// <summary>
        /// In each market you can bought only 3 upgrades.
        /// This mean that you have bought all of them and you need
        /// new Monastery.
        /// </summary>
        MaxUpgrades, 
        
        /// <summary>
        /// Upgrades arent free, you have not enough sources for buying it.
        /// </summary>
        NoSources, 
        
        /// <summary>
        /// You are trying to buy second upgrade for the second time.
        /// You already have second upgrade.
        /// </summary>
        HaveSecondUpgrade };

    public interface IMonastery
    {
        /// <summary>
        /// Invent upgrade for source building of source kind. 
        /// If you have first upgrade, you invent second upgrade. 
        /// Third upgrade doesnt exist.
        /// </summary>
        /// <param name="source">Kind of source building.</param>
        /// <returns>True if upgrade was bought, otherwise false.</returns>
        bool InventUpgrade(SourceBuildingKind source);

        /// <summary>
        /// Returns the reason why you cant invent upgrade or returns
        /// MonasteryError.OK if there is no obstacle to invent upgrade.
        /// </summary>
        /// <param name="source">For which building kind you want invent upgrade.</param>
        /// <returns>MonasteryError.OK if it is OK, or some error.</returns>
        MonasteryError CanInventUpgrade(SourceBuildingKind source);
    }
}
