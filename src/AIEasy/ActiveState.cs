using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class ActiveState
    {
        public ITown activeTown;
        public IRoad activeRoad;
        public SourceKind activeSourceKind;
        public LicenceKind activeLicenceKind;
        public UpgradeKind activeUpgradeKind;
        public SourceBuildingKind activeSourceBuildingKind;
        public byte activeTownPos;
        public IPlayer activePlayer;

        public ActiveState()
        {
            activeLicenceKind = LicenceKind.SecondLicence;
            activeRoad = null;
            activeTown = null;
            activeTownPos = 255;
            activeSourceKind = SourceKind.Count;
            activeUpgradeKind = UpgradeKind.SecondUpgrade;
            activeSourceBuildingKind = SourceBuildingKind.Count;
            activePlayer = null;
        }
    }
}
