using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expanze.MapGeneration;

namespace Expanze
{
    class ForestHexa : Hexa
    {
        public ForestHexa(int value) : base(value, HexaType.Forest)
        {
        }
    }
}
