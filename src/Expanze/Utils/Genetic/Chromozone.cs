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
        Random rnd;

        public Chromozone(Random rnd)
        {
            this.rnd = rnd;

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
                GenerateNewBigGen(loop1);

            ScaleIt();
        }

        internal void GenerateNewBigGen(int id)
        {
            for (int loop2 = 0; loop2 < genes[id].Length; loop2++)
            {
                if (loop2 == 0)
                    genes[id][loop2] = rnd.Next(Genetic.MAX_MAIN_COEF);
                else
                    genes[id][loop2] = rnd.Next(Genetic.SUM_COEF);
            }
        }

        internal Chromozone(int [][] genes)
        {
            rnd = new Random();
            this.genes = genes;
            ScaleIt();
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

        private void ScaleIt()
        {
            for (int loop1 = 0; loop1 < genes.Length; loop1++)
            {
                if(genes[loop1].Length > 2) // main goal coeficient and two params in one
                {
                    double oldSum = 0;
                    int biggest = 0;
                    int biggestID = -1;
                    for (int loop2 = 1; loop2 < genes[loop1].Length; loop2++)
                    {
                        oldSum += genes[loop1][loop2];
                        if (genes[loop1][loop2] > biggest)
                        {
                            biggest = genes[loop1][loop2];
                            biggestID = loop2;
                        }
                    }

                    int newSum = 0;
                    for (int loop2 = 1; loop2 < genes[loop1].Length; loop2++)
                    {
                        genes[loop1][loop2] = (int) ((genes[loop1][loop2] / oldSum) * Genetic.SUM_COEF);
                        newSum += genes[loop1][loop2];
                    }

                    if (newSum < Genetic.SUM_COEF)
                    {
                        genes[loop1][biggestID] += Genetic.SUM_COEF - newSum;
                    }
                }
            }
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

        internal double DistanceTo(Chromozone b)
        {
            double chromDistance = 0.0;
            for (int loop1 = 0; loop1 < genes.Length; loop1++)
            {
                for (int loop2 = 0; loop2 < genes[loop1].Length; loop2++)
                {
                    int genDistance = Math.Abs(genes[loop1][loop2] - b.genes[loop1][loop2]);

                    if (loop2 == 0)
                    {
                        chromDistance += genDistance;
                    }
                    else
                    {
                        chromDistance += 5 * genDistance;
                    }
                }
            }
            return chromDistance;
        }

        internal void SetProbability(double p)
        {
            probability = p;
        }

        internal double GetProbability() { return probability; }
    }
}
