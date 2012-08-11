using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class MountainsHexa : HexaModel
    {
        public MountainsHexa(int value, bool secretKind, bool secretProductivity) : base(value, HexaKind.Mountains, secretKind, secretProductivity, SourceKind.Ore, SourceBuildingKind.Mine, Settings.costMine)
        {
        }
    }
}
