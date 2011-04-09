using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class RaiseSources : CompositeGoal
    {
        ISourceAll source;

        public RaiseSources(IMapController map, ISourceAll source)
            : base(map)
        {
            this.source = source;
            Init();
        }

        public override void Init()
        {
            if (!source.HasPlayerSources(map.GetPlayerMe()))
            {
                AddSubgoal(new ChangeSourcesAtom(map, source));
            }
        }

        public override GoalState Process()
        {
            GoalState state = base.Process();

            return state;
        }
    }
}
