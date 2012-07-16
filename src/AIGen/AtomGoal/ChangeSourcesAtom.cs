using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIGen
{
    class ChangeSourcesAtom : AtomGoal
    {
        List<ISourceAll> sourceList;

        public ChangeSourcesAtom(IMapController map, List<ISourceAll> sourceList, int depth)
            : base(map, depth, "Change sources")
        {
            this.sourceList = sourceList;
        }

        public override GoalState Process()
        {
            if (map.ChangeSourcesFor(sourceList))
            {
                Log(GoalState.Completed);
                return GoalState.Completed;
            }
            else
            {
                Log(GoalState.Active);
                return GoalState.Active;
                //map.Log("goal", "ChangeSourcesAtom - failed > " + map.GetLastError());
                //return GoalState.Failed;
            }
        }
    }
}
