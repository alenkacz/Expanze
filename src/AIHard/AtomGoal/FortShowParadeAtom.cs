using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class FortShowParadeAtom : Goal
    {
        public FortShowParadeAtom(IMapController map, int depth)
            : base(map, depth)
        {
        }

        public override GoalState Process()
        {
            if (map.ShowParade())
            {
                map.Log("goal", "ShowParade - succes");
                return GoalState.Succesed;
            } else
            {
                map.Log("goal", "ShowParade - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}
