using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class InventUpgradeAtom : AtomGoal
    {
        SourceBuildingKind kind;

        public InventUpgradeAtom(IMapController map, SourceBuildingKind kind, int depth)
            : base(map, depth, "Invent upgrade")
        {
            this.kind = kind;
        }

        public override GoalState Process()
        {
            if (map.InventUpgrade(kind))
            {
                Log(GoalState.Succesed);
                return GoalState.Succesed;
            }
            else
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
        }
    }
}
