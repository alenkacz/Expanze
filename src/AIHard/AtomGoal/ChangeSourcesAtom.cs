using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class ChangeSourcesAtom : Goal
    {
        List<ISourceAll> sourceList;

        public ChangeSourcesAtom(IMapController map, List<ISourceAll> sourceList)
            : base(map)
        {
            this.sourceList = sourceList;
        }

        public override GoalState Process()
        {
            if (map.ChangeSourcesFor(sourceList))
            {
                map.Log("goal", "ChangeSourcesAtom - succes");
                return GoalState.Succesed;
            }
            else
            {
                return GoalState.Active;
                //map.Log("goal", "ChangeSourcesAtom - failed > " + map.GetLastError());
                //return GoalState.Failed;
            }
        }
    }
}
