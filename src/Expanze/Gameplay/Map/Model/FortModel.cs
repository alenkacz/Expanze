using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay
{
    class PlayerPromptItem : PromptItem
    {
        Player player;

        public PlayerPromptItem(Player player, String title, String description, ISourceAll source, bool isSourceCost, Texture2D icon)
            : base(title, description, source, isSourceCost, true, icon)
        {
            this.player = player;
            RealCost = false;
        }

        public override void DrawIcon(Microsoft.Xna.Framework.Vector2 iconPosition, bool isMouseOver)
        {
            base.DrawIcon(iconPosition, isMouseOver);
            Texture2D playerIcon = GameResources.Inst().GetHudTexture(HUDTexture.PlayerColor);
            GameState.spriteBatch.Draw(playerIcon, new Vector2(iconPosition.X + getIcon().Width - playerIcon.Width - 5, iconPosition.Y + getIcon().Height - playerIcon.Height - 5), player.GetColor());
        }

        public override void Execute()
        {
            GameState.map.GetMapController().StealSources(player.GetName());
            base.Execute();
        }

        public override string TryExecute()
        {
            return base.TryExecute();
        }
    }

    class FortModel : SpecialBuilding, IFort
    {
        int townID; // where is this building
        int hexaID;
        bool playerPrompt;
        SourceAll costParade;   // it is higher and higher
        SourceAll costSources;
        SourceAll costCaptureHexa;

        public FortModel(Player playerOwner, int townID, int hexaID) : base(playerOwner)
        {
            this.townID = townID;
            this.hexaID = hexaID;
            playerPrompt = false;
            costParade = Settings.costFortParade;
            costSources = Settings.costFortSources;
            costCaptureHexa = Settings.costFortCapture;
        }

        public int GetHexaID() { return hexaID; }

        public override Texture2D GetIconActive()
        {
            return GameResources.Inst().GetHudTexture(HUDTexture.IconFortActive);
        }

        public override Texture2D GetIconPassive()
        {
            return GameResources.Inst().GetHudTexture(HUDTexture.IconFort);
        }

        public override void SetPromptWindow(PromptWindow.Mod mod)
        {
            if (!playerPrompt)
            {
                PromptWindow win = PromptWindow.Inst();
                GameResources res = GameResources.Inst();
                win.Show(mod, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUILD_FORT), true);
                
                if(!Settings.banFortCaptureHexa)
                    win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 0, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_CAPTURE), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_CAPTURE), costCaptureHexa, true, res.GetHudTexture(HUDTexture.IconFortCapture)));
                if(!Settings.banFortCrusade)
                    win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 1, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_CRUSADE), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_CRUSADE), Settings.costFortCrusade, true, res.GetHudTexture(HUDTexture.IconFortCrusade)));          
                if (!Settings.banFortStealSources)
                    win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 2, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_SOURCES), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES), costSources, true, res.GetHudTexture(HUDTexture.IconFortSources)));
                if (!Settings.banFortParade)
                    win.AddPromptItem(new SpecialBuildingPromptItem(townID, hexaID, UpgradeKind.FirstUpgrade, 3, this, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_PARADE), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_PARADE), costParade, true, res.GetHudTexture(HUDTexture.IconFortParade)));
            }
            playerPrompt = false;
        }

        protected override void ApplyEffect(UpgradeKind upgradeKind, int upgradeNumber)
        {
            PromptWindow win = PromptWindow.Inst();
            GameResources res = GameResources.Inst();
            GameMaster gm = GameMaster.Inst();

            upgradeCount--;
            switch (upgradeKind)
            {
                case UpgradeKind.FirstUpgrade:
                    switch (upgradeNumber)
                    {
                        case 0 :
                            //costCaptureHexa += new SourceAll(30);
                            gm.GetActivePlayer().AddSources(GetUpgradeCost(upgradeKind, upgradeNumber), TransactionState.TransactionStart);
                            gm.GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            gm.GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            HexaModel.SetHexaFort(this);
                            gm.SetFortState(EFortState.CapturingHexa);
                            win.Deactive();
                            break;

                        case 1:
                            gm.GetActivePlayer().AddSources(GetUpgradeCost(upgradeKind, upgradeNumber), TransactionState.TransactionStart);
                            gm.GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            gm.GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            HexaModel.SetHexaFort(this);
                            gm.SetFortState(EFortState.DestroyingHexa);
                            win.Deactive();
                            break;

                        case 2 :
                            //costSources += new SourceAll(30);
                            gm.GetActivePlayer().AddSources(GetUpgradeCost(upgradeKind, upgradeNumber), TransactionState.TransactionStart);
                            gm.GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            gm.GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            win.Show(PromptWindow.Mod.Buyer, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_SOURCES), true);
                            for (int loop1 = 0; loop1 < gm.GetPlayerCount(); loop1++)
                            {
                                if (gm.GetPlayer(loop1) != gm.GetActivePlayer())
                                {
                                    win.AddPromptItem(new PlayerPromptItem(gm.GetPlayer(loop1), gm.GetPlayer(loop1).GetName(), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES_CHOISING_PLAYER), gm.GetPlayer(loop1).GetSource(), false, res.GetHudTexture(HUDTexture.IconTown)));
                                }
                            }
                            playerPrompt = true;
                            break;

                        case 3 :
                            gm.GetActivePlayer().AddSources(GetUpgradeCost(upgradeKind, upgradeNumber), TransactionState.TransactionStart);
                            gm.GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            gm.GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            ShowParade();
                            //win.Deactive();
                            break;
                    }
                    break;
                case UpgradeKind.SecondUpgrade:
                    break;
            }
        }

        public override SourceAll GetUpgradeCost(UpgradeKind upgradeKind, int upgradeNumber)
        {
            if (upgradeKind == UpgradeKind.FirstUpgrade)
            {
                switch (upgradeNumber)
                {
                    case 0: return costCaptureHexa;
                    case 1: return Settings.costFortCrusade;
                    case 2: return costSources;
                    case 3: return costParade;
                }
            }
            return new SourceAll(0);
        }

        public static BuildingPromptItem GetPromptItemBuildFort(int townID, int hexaID)
        {
            return new BuildingPromptItem(townID, hexaID, BuildingKind.FortBuilding, Strings.Inst().GetString(TextEnum.PROMPT_TITLE_WANT_TO_BUILD_FORT), Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUILD_FORT), Settings.costFort, true, GameResources.Inst().GetHudTexture(HUDTexture.IconFort));
        }

        public bool ShowParade()
        {
            return GameState.map.GetMapController().ShowParade();
        }

        public ParadeError CanShowParade()
        {
            return GameState.map.GetMapController().CanShowParade();
        }

        public DestroySourcesError CanStealSources(String playerName)
        {
            return GameState.map.GetMapController().CanStealSources(playerName);
        }

        public bool StealSources(String playerName)
        {
            return GameState.map.GetMapController().StealSources(playerName);
        }

        public DestroyHexaError CanDestroyHexa(int hexaID)
        {
            return GameState.map.GetMapController().CanDestroyHexa(hexaID, this);
        }

        public bool DestroyHexa(int hexaID)
        {
            return GameState.map.GetMapController().DestroyHexa(hexaID, this);
        }

        public CaptureHexaError CanCaptureHexa(int hexaID)
        {
            return GameState.map.GetMapController().CanCaptureHexa(hexaID, this);
        }

        public bool CaptureHexa(int hexaID)
        {
            return GameState.map.GetMapController().CaptureHexa(hexaID, this);
        }
    }
}
