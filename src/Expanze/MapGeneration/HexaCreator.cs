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
                    return new CornfieldHexa();
                case Settings.Types.Desert:
                    return new DesertHexa();
                case Settings.Types.Forest:
                    return new ForestHexa();
                case Settings.Types.Mountains:
                    return new MountainsHexa();
                case Settings.Types.Pasture:
                    return new PastureHexa();
                case Settings.Types.Stone:
                    return new StoneHexa();
                default:
                    return null;
            }
        }
    }
}
