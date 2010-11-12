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
        public static HexaModel create(HexaType type, int number)
        {
            switch (type)
            {
                case HexaType.Cornfield:
                    return new CornfieldHexa(number);
                case HexaType.Desert:
                    return new DesertHexa(number);
                case HexaType.Forest:
                    return new ForestHexa(number);
                case HexaType.Mountains:
                    return new MountainsHexa(number);
                case HexaType.Pasture:
                    return new PastureHexa(number);
                case HexaType.Stone:
                    return new StoneHexa(number);
                case HexaType.Water:
                    return new WaterHexa();
                case HexaType.Nothing:
                    return null;
                default:
                    throw new Exception("XML file with map is broken.");
            }
        }
    }
}
