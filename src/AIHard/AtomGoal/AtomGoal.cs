using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{
    abstract class AtomGoal : Goal
    {
        public AtomGoal(IMapController map, int depth, string name)
            : base(map, depth, name)
        {
        }

        protected void Log(GoalState state)
        {
            string logMsg = "";

            logMsg += "@ " + name + " > ";

            switch (state)
            {
                case GoalState.Active: logMsg += "active"; break;
                case GoalState.Failed: logMsg += "fail - " + map.GetLastError(); break;
                case GoalState.Succesed: logMsg += "succes"; break;
            }

            for (int loop1 = 0; loop1 < depth; loop1++)
            {
                logMsg = "  " + logMsg;
            }

            Log(logMsg);
        }
    }
}
