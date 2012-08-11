using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class CornfieldHexa : HexaModel
    {
        public CornfieldHexa(int value, bool secretKind, bool secretProductivity) : base(value, HexaKind.Cornfield, secretKind, secretProductivity, SourceKind.Corn, SourceBuildingKind.Mill, Settings.costMill)
        {
        }
    }
}
