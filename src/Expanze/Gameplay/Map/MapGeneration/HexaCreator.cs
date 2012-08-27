using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    /// <summary>
    /// Hexa creator factory
    /// </summary>
    class HexaCreator
    {
        public static HexaModel create(string typeS, string numberS, bool secretKind, bool secretProductivity)
        {
            HexaKind type = decideType(typeS);
            int number = decideProductivity(numberS);

            switch (type)
            {
                case HexaKind.Cornfield:
                    return new CornfieldHexa(number, secretKind, secretProductivity);
                case HexaKind.Desert:
                    return new DesertHexa(secretKind, secretProductivity);
                case HexaKind.Forest:
                    return new ForestHexa(number, secretKind, secretProductivity);
                case HexaKind.Mountains:
                    return new MountainsHexa(number, secretKind, secretProductivity);
                case HexaKind.Pasture:
                    return new PastureHexa(number, secretKind, secretProductivity);
                case HexaKind.Stone:
                    return new StoneHexa(number, secretKind, secretProductivity);
                case HexaKind.Water:
                    return new WaterHexa(secretKind, secretProductivity);
                case HexaKind.Nothing:
                    return null;
                default:
                    throw new Exception("XML file with map is broken.");
            }
        }

        public static int decideProductivity(string productivity)
        {
            double rndNumber = GameMaster.Inst().GetRandomNumber();
            if (productivity == "+")
            {
                if (rndNumber < 0.25) return 16;
                if (rndNumber < 0.75) return 20;
                return 24;
            }
            else if (productivity == "-")
            {
                if (rndNumber < 0.25) return 8;
                if (rndNumber < 0.75) return 12;
                return 16;
            }
            else if (productivity == "?")
            {
                if (rndNumber < 1.0 / 5.0) return 8;
                if (rndNumber < 2.0 / 5.0) return 12;
                if (rndNumber < 3.0 / 5.0) return 16;
                if (rndNumber < 4.0 / 5.0) return 20;
                return 24;
            }
            else
            {
                return Convert.ToInt32(productivity);
            }
        }

        /// <summary>
        /// Returns type of the hexas
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HexaKind decideType(String type)
        {
            double rndNumber = GameMaster.Inst().GetRandomNumber();
            switch (type)
            {
                case "cornfield":
                    return HexaKind.Cornfield;
                case "forest":
                    return HexaKind.Forest;
                case "desert":
                    return HexaKind.Desert;
                case "mountains":
                case "mountain":
                    return HexaKind.Mountains;
                case "pasture":
                    return HexaKind.Pasture;
                case "stone":
                    return HexaKind.Stone;
                case "water":
                    return HexaKind.Water;
                case "2":
                    return (rndNumber < 0.5 ? HexaKind.Stone : HexaKind.Mountains);

                case "2+":
                    if (rndNumber < 1.0 / 3.0) return HexaKind.Stone;
                    if (rndNumber < 2.0 / 3.0) return HexaKind.Mountains;
                    return HexaKind.Desert;

                case "3":
                    if (rndNumber < 1.0 / 3.0) return HexaKind.Forest;
                    if (rndNumber < 2.0 / 3.0) return HexaKind.Cornfield;
                    return HexaKind.Pasture;

                case "?":
                    if (rndNumber < 1.0 / 5.0) return HexaKind.Forest;
                    if (rndNumber < 2.0 / 5.0) return HexaKind.Cornfield;
                    if (rndNumber < 3.0 / 5.0) return HexaKind.Pasture;
                    if (rndNumber < 4.0 / 5.0) return HexaKind.Stone;
                    return HexaKind.Mountains;

                case "?+":
                    if (rndNumber < 1.0 / 6.0) return HexaKind.Forest;
                    if (rndNumber < 2.0 / 6.0) return HexaKind.Cornfield;
                    if (rndNumber < 3.0 / 6.0) return HexaKind.Pasture;
                    if (rndNumber < 4.0 / 6.0) return HexaKind.Stone;
                    if (rndNumber < 5.0 / 6.0) return HexaKind.Mountains;
                    return HexaKind.Desert;

                case "nothing":
                case "space":
                    return HexaKind.Nothing;
                default:
                    return HexaKind.Null;
            }
        }


    }
}
