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
        public static HexaModel create(HexaKind type, int number)
        {
            switch (type)
            {
                case HexaKind.Cornfield:
                    return new CornfieldHexa(number);
                case HexaKind.Desert:
                    return new DesertHexa();
                case HexaKind.Forest:
                    return new ForestHexa(number);
                case HexaKind.Mountains:
                    return new MountainsHexa(number);
                case HexaKind.Pasture:
                    return new PastureHexa(number);
                case HexaKind.Stone:
                    return new StoneHexa(number);
                case HexaKind.Water:
                    return new WaterHexa();
                case HexaKind.Nothing:
                    return null;
                default:
                    throw new Exception("XML file with map is broken.");
            }
        }
    }
}
