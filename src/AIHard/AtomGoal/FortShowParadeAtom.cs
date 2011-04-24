using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
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
                Log(GoalState.Succesed);
                return GoalState.Succesed;
            } else
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
        }
    }
}
