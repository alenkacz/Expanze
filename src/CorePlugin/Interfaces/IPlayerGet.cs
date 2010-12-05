using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public interface IPlayerGet
    {
        UpgradeKind GetSourceBuildingUpgrade(SourceBuildingKind kind);
        ISourceAll GetSource();
    }
}
