using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class FortShowParadeAtom : Goal
    {
        public FortShowParadeAtom(IMapController map)
            : base(map)
        {
        }

        public override GoalState Process()
        {
            if (map.ShowParade())
            {
                map.Log("goal", "ShowParade");
                return GoalState.Succesed;
            } else
            {
                map.Log("goal", "ShowParade - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}
