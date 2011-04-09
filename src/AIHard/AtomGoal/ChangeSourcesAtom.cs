using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class ChangeSourcesAtom : Goal
    {
        ISourceAll source;

        public ChangeSourcesAtom(IMapController map, ISourceAll source)
            : base(map)
        {
            this.source = source;
        }

        public override GoalState Process()
        {
            if (map.ChangeSourcesFor(source))
            {
                map.Log("goal.txt", "ChangeSourcesAtom - succes");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal.txt", "ChangeSourcesAtom - failed");
                return GoalState.Failed;
            }
        }
    }
}
