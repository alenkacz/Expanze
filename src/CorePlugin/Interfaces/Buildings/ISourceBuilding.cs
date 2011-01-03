using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum SourceBuildingError
    {
        /// <summary>
        /// There is no problem with upgrading source building.
        /// </summary>
        OK,

        /// <summary>
        /// Upgrades arent free, you have not enough sources for buying it.
        /// </summary>
        NoSources,

        /// <summary>
        /// You are trying to buy second upgrade for the second time.
        /// You already have second upgrade.
        /// </summary>
        HaveSecondUpgrade
    };

    public interface ISourceBuilding
    {
        /// <summary>
        /// Upgrade source building to next level if you have invented upgrade in monastery and you have sources on it
        /// </summary>
        /// <returns>If building was upgraded.</returns>
        bool Upgrade();

        /// <summary>
        /// Find out the reason why you cant upgrade source building or
        /// returns SourceBuildingError.OK when there is no problem.
        /// </summary>
        /// <returns>SourceBuildingError.OK if it is ok or kind of error which occured.</returns>
        SourceBuildingError CanUpgrade();
    }
}
