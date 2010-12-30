using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public interface ISourceBuilding
    {
        /// <summary>
        /// Upgrade source building to next level if you have invented upgrade in monastery and you have sources on it
        /// </summary>
        /// <returns>If building was upgraded.</returns>
        bool Upgrade();
    }
}
