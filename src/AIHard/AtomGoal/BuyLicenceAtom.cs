using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuyLicenceAtom : AtomGoal
    {
        SourceKind kind;

        public BuyLicenceAtom(IMapController map, SourceKind kind, int depth)
            : base(map, depth, "Buy licence")
        {
            this.kind = kind;
        }

        public override GoalState Process()
        {
            if (map.BuyLicence(kind))
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
