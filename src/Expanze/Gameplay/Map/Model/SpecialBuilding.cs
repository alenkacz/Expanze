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
    }

    abstract class SpecialBuilding
    {
        protected bool[] upgradeFirst;
        protected bool[] upgradeSecond;

        public SpecialBuilding()
        {
            upgradeFirst = new bool[5];
            upgradeSecond = new bool[5];

            for (int loop1 = 0; loop1 < upgradeFirst.Length; loop1++)
            {
                upgradeFirst[loop1] = false;
                upgradeSecond[loop1] = false;
            }
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
        }

        abstract public void setPromptWindow();
        abstract public SourceAll getUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber);
    }
}
