using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum BuyingUpgradeError { OK, NoSources, 
                                     MaxUpgrades,
                                     NoUpgrade}

    public interface ISpecialBuildingGet
    {
        BuyingUpgradeError CanActivePlayerBuyUpgrade(UpgradeKind upgradeKind, int upgradeNumber);
    }
}
