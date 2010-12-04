using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;

namespace Expanze.Gameplay
{
    class SpecialBuildingPromptItem : PromptItem
    {
        int townID;         /// where is special building?
        int hexaID;         /// where is special building?
        UpgradeKind upgradeKind;    /// first or second
        int upgradeNumber;  /// which from 5 upgraded player wants to upgrade

        public SpecialBuildingPromptItem(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber, String title, String description, SourceAll cost, Texture2D icon)
            : base(title, description, cost, icon)
        {
            this.townID = townID;
            this.hexaID = hexaID;
            this.upgradeKind = upgradeKind;
            this.upgradeNumber = upgradeNumber;
        }

        public override void Execute()
        {
            GameState.map.GetMapController().BuyUpgradeInSpecialBuilding(townID, hexaID, upgradeKind, upgradeNumber);
        }

        public override string TryExecute()
        {
            GameMaster gm = GameMaster.getInstance();
            Town town = GameState.map.GetTownByID(townID);
            SpecialBuilding building = town.getSpecialBuilding(hexaID);

            BuyingUpgradeError error = building.CanActivePlayerBuyUpgrade(upgradeKind, upgradeNumber);
            switch (error)
            {
                case BuyingUpgradeError.NoSources: return "";
                case BuyingUpgradeError.MaxUpgrades: return Strings.ALERT_TITLE_MAX_UPGRADES;
                case BuyingUpgradeError.NoUpgrade: return Strings.ALERT_TITLE_NO_UPGRADE;
            }

            return base.TryExecute();
        }
    }

    abstract class SpecialBuilding : ISpecialBuildingGet
    {
        protected bool[] upgradeFirst;
        protected bool[] upgradeSecond;
        const int upgradeMax = 3;   /// upgradeCount limit
        int upgradeCount;           /// how many upgrades player has bought in this building?

        public SpecialBuilding()
        {
            upgradeFirst = new bool[5];
            upgradeSecond = new bool[5];
            upgradeCount = 0;

            for (int loop1 = 0; loop1 < upgradeFirst.Length; loop1++)
            {
                upgradeFirst[loop1] = false;
                upgradeSecond[loop1] = false;
            }
        }

        public bool GetIsUpgrade(UpgradeKind kind, int upgradeNumber)
        {
            switch (kind)
            {
                case UpgradeKind.FirstUpgrade:
                    return upgradeFirst[upgradeNumber];
                case UpgradeKind.SecondUpgrade:
                    return upgradeSecond[upgradeNumber];
            }

            return false;
        }

        public void BuyUpgrade(UpgradeKind kind, int upgradeNumber)
        {
            switch (kind)
            {
                case UpgradeKind.FirstUpgrade:
                    upgradeFirst[upgradeNumber] = true;
                    break;
                case UpgradeKind.SecondUpgrade:
                    upgradeSecond[upgradeNumber] = true;
                    break;
            }
            upgradeCount++;
        }

        abstract public void setPromptWindow();
        abstract public SourceAll getUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber);

        public virtual BuyingUpgradeError CanActivePlayerBuyUpgrade(UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.getInstance();
            Player activePlayer = gm.getActivePlayer();

            if (upgradeCount == upgradeMax)
                return BuyingUpgradeError.MaxUpgrades;

            if (!getUpgradeCost(upgradeKind, upgradeNumber).HasPlayerSources(activePlayer))
                return BuyingUpgradeError.NoSources;

            return BuyingUpgradeError.OK;
        }
    }
}
