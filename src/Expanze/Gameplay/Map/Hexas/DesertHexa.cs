using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class DesertHexa : HexaModel
    {
        public DesertHexa(bool secretKind, bool secretProductivity) : base(0, HexaKind.Desert, secretKind, secretProductivity, SourceKind.Null, SourceBuildingKind.Count, new SourceAll(0))
        {
        }
    }
}
