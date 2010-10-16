using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    /// <summary>
    /// Hexa creator factory
    /// </summary>
    class HexaCreator
    {
        public static Hexa create(Settings.Types type, int number)
        {
            switch (type)
            {
                case Settings.Types.Cornfield:
                    return new CornfieldHexa(number);
                case Settings.Types.Desert:
                    return new DesertHexa(number);
                case Settings.Types.Forest:
                    return new ForestHexa(number);
                case Settings.Types.Mountains:
                    return new MountainsHexa(number);
                case Settings.Types.Pasture:
                    return new PastureHexa(number);
                case Settings.Types.Stone:
                    return new StoneHexa(number);
                case Settings.Types.Water:
                    return new WaterHexa();
                case Settings.Types.Nothing:
                    return null;
                default:
                    throw new Exception("XML file with map is broken.");
            }
        }
    }
}
