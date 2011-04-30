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

        public Chromozone(int length, Random rnd)
        {
            genes = new double[length];

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

            double delta = fitness - second.fitness;
            return (delta < 0) ? 1 : ((delta > 0) ? -1 : 0);
        }

        #endregion

        internal double GetFitness()
        {
            return fitness;
        }

        internal void Log()
        {
            string msg = fitness + " > ";
            foreach (double d in genes)
            {
                msg += d + "; ";
            }
            Logger.Inst().Log("chromozone.txt", msg);
        }
    }
}
