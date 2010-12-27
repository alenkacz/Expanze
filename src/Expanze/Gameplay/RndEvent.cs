using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay
{
    public class RndEvent
    {
        bool isPositive;    // false it is negative
        HexaKind hexaKind;  // which hexa is effects by event

        private RndEvent(HexaKind hexaKind, bool isPositive)
        {
            this.isPositive = isPositive;
            this.hexaKind = hexaKind;

            string title = "";
            string description = "";
            Texture2D icon = null; ;
            if (isPositive)
            {
                switch (hexaKind)
                {
                    case HexaKind.Cornfield :
                        title = Strings.MESSAGE_TITLE_MIRACLE;
                        description = Strings.MESSAGE_DESCRIPTION_MIRACLE_CORNFIELD;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconMill);
                        break;
                    case HexaKind.Pasture :
                        title = Strings.MESSAGE_TITLE_MIRACLE;
                        description = Strings.MESSAGE_DESCRIPTION_MIRACLE_PASTURE;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconStepherd);
                        break;
                    case HexaKind.Stone :
                        title = Strings.MESSAGE_TITLE_MIRACLE;
                        description = Strings.MESSAGE_DESCRIPTION_MIRACLE_STONE;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconQuarry);
                        break;
                    case HexaKind.Forest :
                        title = Strings.MESSAGE_TITLE_MIRACLE;
                        description = Strings.MESSAGE_DESCRIPTION_MIRACLE_FOREST;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconSaw);
                        break;
                    case HexaKind.Mountains :
                        title = Strings.MESSAGE_TITLE_MIRACLE;
                        description = Strings.MESSAGE_DESCRIPTION_MIRACLE_MOUNTAINS;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconMine);
                        break;
                }
            }
            else
            {
                switch (hexaKind)
                {
                    case HexaKind.Cornfield:
                        title = Strings.MESSAGE_TITLE_DISASTER;
                        description = Strings.MESSAGE_DESCRIPTION_DISASTER_CORNFIELD;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconMill);
                        break;
                    case HexaKind.Pasture:
                        title = Strings.MESSAGE_TITLE_DISASTER;
                        description = Strings.MESSAGE_DESCRIPTION_DISASTER_PASTURE;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconStepherd);
                        break;
                    case HexaKind.Stone:
                        title = Strings.MESSAGE_TITLE_DISASTER;
                        description = Strings.MESSAGE_DESCRIPTION_DISASTER_STONE;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconQuarry);
                        break;
                    case HexaKind.Forest:
                        title = Strings.MESSAGE_TITLE_DISASTER;
                        description = Strings.MESSAGE_DESCRIPTION_DISASTER_FOREST;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconSaw);
                        break;
                    case HexaKind.Mountains:
                        title = Strings.MESSAGE_TITLE_DISASTER;
                        description = Strings.MESSAGE_DESCRIPTION_DISASTER_MOUNTAINS;
                        icon = GameResources.Inst().getHudTexture(HUDTexture.IconMine);
                        break;
                }
            }

            GameState.message.Show(title, description, icon);
        }

        public HexaKind getHexaKind() { return hexaKind; }
        public bool getIsPositive() { return isPositive; }

        public static RndEvent getRandomEvent(Random randomNumber)
        {
            HexaKind hexa = (HexaKind)(randomNumber.Next() % 5);
            bool positiveEvent = (randomNumber.Next() % 2) == 0;
            return new RndEvent(hexa, positiveEvent);
        }
    }
}
