﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze
{
    class DesertHexa : HexaModel
    {
        public DesertHexa(int value) : base(value, HexaKind.Desert, new SourceAll(0))
        {
        }
    }
}
