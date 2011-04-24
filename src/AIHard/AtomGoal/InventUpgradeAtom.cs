using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class InventUpgradeAtom : Goal
    {
        SourceBuildingKind kind;

        public InventUpgradeAtom(IMapController map, SourceBuildingKind kind, int depth)
            : base(map, depth)
        {
            this.kind = kind;
        }

        public override GoalState Process()
        {
            if (map.InventUpgrade(kind))
            {
                map.Log("goal", "InventUpgradeAtom - sucess");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "InventUpgradeAtom - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}
