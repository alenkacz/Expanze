using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class ForestHexa : HexaModel
    {
        public ForestHexa(int value, bool secretKind, bool secretProductivity) : base(value, HexaKind.Forest, secretKind, secretProductivity, SourceKind.Wood, SourceBuildingKind.Saw, Settings.costSaw)
        {
        }
    }
}
