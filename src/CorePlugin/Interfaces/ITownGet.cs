using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum BuildingKind { NoBuilding, SourceBuilding }
    public interface ITownGet
    {
        int getTownID();
        ISourceAll getCost();
    }
}
