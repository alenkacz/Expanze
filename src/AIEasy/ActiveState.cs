﻿using System;
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

        public ActiveState()
        {
            activeLicenceKind = LicenceKind.NoLicence;
            activeRoad = null;
            activeTown = null;
            activeTownPos = 255;
            activeSourceKind = SourceKind.Count;
            activeUpgradeKind = UpgradeKind.NoUpgrade;
            activeSourceBuildingKind = SourceBuildingKind.Count;
        }
    }
}
