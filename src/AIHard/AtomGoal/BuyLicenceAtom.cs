using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    class BuyLicenceAtom : Goal
    {
        SourceKind kind;

        public BuyLicenceAtom(IMapController map, SourceKind kind, int depth)
            : base(map, depth, "Buy licence Atom")
        {
            this.kind = kind;
        }

        public override GoalState Process()
        {
            if (map.BuyLicence(kind))
            {
                map.Log("goal", "BuyLicenceAtom - sucess");
                return GoalState.Succesed;
            }
            else
            {
                map.Log("goal", "BuyLicenceAtom - failed > " + map.GetLastError());
                return GoalState.Failed;
            }
        }
    }
}
