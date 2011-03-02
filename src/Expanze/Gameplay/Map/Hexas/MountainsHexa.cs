using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class MountainsHexa : HexaModel
    {
        public MountainsHexa(int value) : base(value, HexaKind.Mountains, SourceKind.Ore, SourceBuildingKind.Mine, Settings.costMine)
        {
        }
    }
}
