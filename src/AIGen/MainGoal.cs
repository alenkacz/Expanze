using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIGen
{
    class MainGoal
    {
        public CompositeGoal goal;
        public double desirabilityCoef;

        public MainGoal(CompositeGoal goal, double desirabilityCoef)
        {
            this.goal = goal;
            this.desirabilityCoef = desirabilityCoef;
        }
    }
}
