using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    class DesertHexa : Hexa
    {
        public DesertHexa(int value) : base(value, Settings.Types.Desert)
        {
        }
    }
}
