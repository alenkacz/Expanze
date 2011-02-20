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
        SpecialBuilding building;

        public SpecialBuildingPromptItem(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber, SpecialBuilding building, String title, String description, SourceAll source, bool isSourceCost, Texture2D icon)
            : base(title, description, source, isSourceCost, false, icon)
        {
            this.townID = townID;
            this.hexaID = hexaID;
            this.upgradeKind = upgradeKind;
            this.upgradeNumber = upgradeNumber;
            this.building = building;
        }

        public override void Execute()
        {
            int tempItem = PromptWindow.Inst().GetActiveItem();
            building.SetPromptWindow(PromptWindow.Mod.Buyer);
            PromptWindow.Inst().SetActiveItem(tempItem);
            
            GameState.map.GetMapController().BuyUpgradeInSpecialBuilding(townID, hexaID, upgradeKind, upgradeNumber);            
        }

        public override string TryExecute()
        {
            GameMaster gm = GameMaster.Inst();
            TownModel town = GameState.map.GetTownByID(townID);
            SpecialBuilding building = town.GetSpecialBuilding(hexaID);

            BuyingUpgradeError error = building.CanActivePlayerBuyUpgrade(upgradeKind, upgradeNumber);
            switch (error)
            {
                case BuyingUpgradeError.NoSources: return "";
                case BuyingUpgradeError.MaxUpgrades: return Strings.ALERT_TITLE_MAX_UPGRADES;
                case BuyingUpgradeError.NoUpgrade: return Strings.ALERT_TITLE_NO_UPGRADE;
                case BuyingUpgradeError.YouAlreadyHaveSecondUpgrade: return ""; // Strings.ALERT_TITLE_ALREADY_HAVE_SECOND_UPGRADE;
            }

            return base.TryExecute();
        }
    }

    abstract class SpecialBuilding : ISpecialBuildingGet
    {
        const int upgradeMax = 3;             /// upgradeCount limit
        protected int upgradeCount;           /// how many upgrades player has bought in this building?
        protected Player owner;               /// owner of that building
            
        public SpecialBuilding(Player playerOwner)
        {
            upgradeCount = 0;
            owner = playerOwner;
        }

        public void BuyUpgrade(UpgradeKind kind, int upgradeNumber)
        {
            upgradeCount++;
            ApplyEffect(kind, upgradeNumber);
        }

        abstract public void SetPromptWindow(PromptWindow.Mod mod);
        abstract public SourceAll GetUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber);
        abstract public Texture2D GetIconActive();
        abstract public Texture2D GetIconPassive();
        abstract protected void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber);
        

        public virtual BuyingUpgradeError CanActivePlayerBuyUpgrade(UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.Inst();
            Player activePlayer = gm.GetActivePlayer();

            if (upgradeCount == upgradeMax)
                return BuyingUpgradeError.MaxUpgrades;    

            if (!GetUpgradeCost(upgradeKind, upgradeNumber).HasPlayerSources(activePlayer))
                return BuyingUpgradeError.NoSources;

            return BuyingUpgradeError.OK;
        }
    }
}
