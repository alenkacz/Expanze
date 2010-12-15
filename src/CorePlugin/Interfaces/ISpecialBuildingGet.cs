using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum BuyingUpgradeError
    {
        OK, NoSources,
        MaxUpgrades,
        NoUpgrade,
        YouDontHaveFirstUpgrade,
        YouAlreadyHaveSecondUpgrade,
        ThereIsNoBuilding,
        ThereIsNoTown
    }

    public interface ISpecialBuildingGet
    {
        BuyingUpgradeError CanActivePlayerBuyUpgrade(UpgradeKind upgradeKind, int upgradeNumber);
    }
}
