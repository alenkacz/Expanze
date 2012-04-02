using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
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
                Log(GoalState.Completed);
                return GoalState.Completed;
            }
            else
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
        }
    }
}
