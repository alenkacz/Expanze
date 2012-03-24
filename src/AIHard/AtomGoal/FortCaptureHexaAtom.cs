using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class FortCaptureHexaAtom : AtomGoal
    {
        IHexa hexa; 

        public FortCaptureHexaAtom(IMapController map, IHexa hexa, int depth)
            : base(map, depth, "Capture hexa")
        {
            this.hexa = hexa;
        }

        public override GoalState Process()
        {
            if (map.CaptureHexa(hexa))
            {
                Log(GoalState.Completed);
                return GoalState.Completed;
            }
            else
            {
                Log(GoalState.Failed);
                return GoalState.Failed;
            }
        }
    }
}
