using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class RaiseSources : CompositeGoal
    {
        List<ISourceAll> source;

        public RaiseSources(IMapController map, List<ISourceAll> sourceList)
            : base(map)
        {
            this.source = sourceList;
            Init();
        }

        public RaiseSources(IMapController map, PriceKind priceKind)
            : this(map, map.GetPrice(priceKind))
        {
        }

        public RaiseSources(IMapController map, ISourceAll source)
            : base(map)
        {
            List<ISourceAll> sourceList = new List<ISourceAll>();
            sourceList.Add(source);
            this.source = sourceList;
            Init();
        }

        public override void Init()
        {
            //if (!source.HasPlayerSources(map.GetPlayerMe()))
            {
                AddSubgoal(new ChangeSourcesAtom(map, source));
            }
        }

        public override GoalState Process()
        {
            GoalState state = base.Process();

            if (state == GoalState.Failed)
            {
                AddSubgoal(new ChangeSourcesAtom(map, source));
                return GoalState.Active;
            }

            return state;
        }

        public override double GetDesirability()
        {
            return 0.0f;
        }
    }
}
