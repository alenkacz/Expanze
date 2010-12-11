using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

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

            GameState.windowPromt.showAlert("Zázrak", "Ovce snědly jedovatý jetel. Bude jich o polovinu méně. Bla bla je to tak, slečna je. ahoj, jak to jde< dobre to je", GameResources.Inst().getHudTexture(HUDTexture.IconMine));
        }

        public static RndEvent getRandomEvent()
        {
            Random random = new System.Random();
            bool positiveEvent = (random.Next() % 2) == 0;
            HexaKind hexa = (HexaKind)(random.Next() % 5);
            return new RndEvent(hexa, positiveEvent);
        }
    }
}
