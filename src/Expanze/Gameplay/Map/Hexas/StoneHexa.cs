using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class StoneHexa : HexaModel
    {
        public StoneHexa(int value, bool secretKind, bool secretProductivity) : base(value, HexaKind.Stone, secretKind, secretProductivity, SourceKind.Stone, SourceBuildingKind.Quarry, Settings.costQuarry)
        {
        }
    }
}
