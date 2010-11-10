using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public interface ITownGet
    {
        int getTownID();
        ISourceAll getCost();
    }
}
