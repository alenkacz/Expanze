using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class ForestHexa : HexaModel
    {
        public ForestHexa(int value) : base(value, HexaKind.Forest, SourceKind.Wood, SourceBuildingKind.Saw, Settings.costSaw)
        {
        }
    }
}
