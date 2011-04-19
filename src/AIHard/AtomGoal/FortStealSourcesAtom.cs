using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class FortStealSourcesAtom : Goal
    {
        IPlayer player;

        public FortStealSourcesAtom(IMapController map, IPlayer player)
            : base(map)
        {
            this.player = player;
        }

        public override GoalState Process()
        {
            if (map.StealSources(player.GetName()))
            {
                map.Log("goal", "StealSources - succes");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "StealSources - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}
