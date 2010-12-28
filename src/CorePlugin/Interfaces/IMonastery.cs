using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public interface IMonastery
    {
        /// <summary>
        /// Invent upgrade for source building of source kind. 
        /// If you have first upgrade, you invent second upgrade. 
        /// Third upgrade doesnt exist.
        /// </summary>
        /// <param name="source">Kind of source building.</param>
        /// <returns>True if upgrade was bought, otherwise false.</returns>
        bool InventUpgrade(SourceKind source);
    }
}
