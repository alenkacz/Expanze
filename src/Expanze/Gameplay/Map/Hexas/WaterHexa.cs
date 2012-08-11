using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class WaterHexa : HexaModel
    {
        public WaterHexa(bool secretKind, bool secretProductivity) : base(0, HexaKind.Water, secretKind, secretProductivity, SourceKind.Null, SourceBuildingKind.Count, new SourceAll(0))
        {
        }
    }
}
