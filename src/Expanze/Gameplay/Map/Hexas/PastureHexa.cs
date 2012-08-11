using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class PastureHexa : HexaModel
    {
        public PastureHexa(int value, bool secretKind, bool secretProductivity) : base(value, HexaKind.Pasture, secretKind, secretProductivity, SourceKind.Meat, SourceBuildingKind.Stepherd, Settings.costStephard)
        {
        }
    }
}
