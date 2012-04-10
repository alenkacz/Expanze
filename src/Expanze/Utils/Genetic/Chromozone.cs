using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.Utils.Genetic
{
    class Chromozone : IComparable
    {
        int[][] genes;
        double fitness;
        double probability;

        public Chromozone(Random rnd)
        {
            genes = new int[11][];
            genes[0] = new int[4];
            genes[1] = new int[2];
            genes[2] = new int[4];
            genes[3] = new int[5];
            genes[4] = new int[2];
            genes[5] = new int[6];
            genes[6] = new int[6];
            genes[7] = new int[2];
            genes[8] = new int[2];
            genes[9] = new int[2];
            genes[10] = new int[2];

            for (int loop1 = 0; loop1 < genes.Length; loop1++)
                for (int loop2 = 0; loop2 < genes[loop1].Length; loop2++)
                {
                    if(loop2 == 0)
                        genes[loop1][loop2] = rnd.Next(1000);
                    else
                        genes[loop1][loop2] = rnd.Next(1000);
                }
        }

        internal Chromozone(int [][] genes)
        {
            this.genes = genes;
        }

        internal int[][] GetGenes() { return genes; }

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
            Log("chromozone.txt");
        }

        internal void Log(string src)
        {
            string msg = String.Format("{0:0.000}", fitness) + ";;";
            foreach (int[] action in genes)
            {
                foreach(int koef in action)
                    msg += String.Format("{0,3}", koef) + ";";

                msg += ";";
            }
            Logger.Inst().Log(src, msg);
        }

        internal void SetProbability(double p)
        {
            probability = p;
        }

        internal double GetProbability() { return probability; }
    }
}
