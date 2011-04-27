using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.Utils.Genetic
{
    class Chromozone : IComparable
    {
        double[] genes;
        double fitness;

        public Chromozone(int length)
        {
            genes = new double[length];

            Random rnd = new Random();
            for (int loop1 = 0; loop1 < length; loop1++)
                genes[loop1] = rnd.NextDouble();
        }

        internal Chromozone(double[] genes)
        {
            this.genes = genes;
        }

        internal double[] GetGenes() { return genes; }

        internal void SetFitness(double fitness)
        {
            this.fitness = fitness;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            Chromozone second = (Chromozone) obj;

            return (int)(fitness - second.fitness);
        }

        #endregion

        internal double GetFitness()
        {
            return fitness;
        }
    }
}
