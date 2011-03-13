using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlugin
{
    public enum Building { Town, Road, Market, Monastery, Fort, Mill, Stepherd, Quarry, Saw, Mine, Count }

    public interface IPlayer
    {
        UpgradeKind GetMonasteryUpgrade(SourceBuildingKind kind);
        LicenceKind GetMarketLicence(SourceKind kind);

        ISourceAll GetSource();
        ISourceAll GetCollectSourcesNormal();
        int GetBuildingCount(Building building);
        String GetName();
    }
}
