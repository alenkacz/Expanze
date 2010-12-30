using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public interface IPlayer
    {
        UpgradeKind GetSourceBuildingUpgrade(SourceBuildingKind kind);
        ISourceAll GetSource();
        String GetName();
    }
}
