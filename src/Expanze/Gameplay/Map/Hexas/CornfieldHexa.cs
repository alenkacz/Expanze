﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class CornfieldHexa : HexaModel
    {
        public CornfieldHexa(int value) : base(value, HexaKind.Cornfield, SourceKind.Corn, SourceBuildingKind.Mill, Settings.costMill)
        {
        }
    }
}
