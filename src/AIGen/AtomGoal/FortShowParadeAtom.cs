using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class FortShowParadeAtom : AtomGoal
    {
        public FortShowParadeAtom(IMapController map, int depth)
            : base(map, depth, "Show parade")
        {
        }

        public override GoalState Process()
        {
            if (map.ShowParade())
            {
                Log(GoalState.Completed);
                return GoalState.Completed;
            } else
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
        }
    }
}
