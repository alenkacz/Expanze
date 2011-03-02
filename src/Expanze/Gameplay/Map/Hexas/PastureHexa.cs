﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class PastureHexa : HexaModel
    {
        public PastureHexa(int value) : base(value, HexaKind.Pasture, SourceKind.Meat, SourceBuildingKind.Stepherd, Settings.costStephard)
        {
        }
    }
}
