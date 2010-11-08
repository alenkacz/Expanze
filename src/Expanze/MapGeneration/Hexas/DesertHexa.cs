using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expanze.MapGeneration;

namespace Expanze
{
    class DesertHexa : Hexa
    {
        public DesertHexa(int value) : base(value, HexaType.Desert)
        {
        }
    }
}
