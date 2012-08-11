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
        public static HexaModel create(HexaKind type, int number, bool secretKind, bool secretProductivity)
        {
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
    }
}
