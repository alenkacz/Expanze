using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Expanze.Utils.Genetic
{
    class Genetic
    {
        internal const int SUM_COEF = 100;
        internal const int MAX_MAIN_COEF = 1000;

        Chromozone[] population;
        Chromozone theBestOne;
        double fitnessBestOne;
        
        double lastFitness;
        Stopwatch stopwatch;

        int activeChromozomeID;
        int populationSize;
        int elitism;
        double probCrossOver;
        double probMutability;
        int generationNumber;
        double shareSigma;
        double shareAlfa;
        double scaleSigma;
        double extinction;
        bool newBorn;
        Random rnd;
        bool[] zeros;
        int zerosSum;

        public Genetic(int populationSize, double probCrossOver, double probMutability, int elitism, double shareSigma, double shareAlfa, double scaleSigma, bool newBorn, bool [] zeros)
        {
            rnd = new Random();
            this.shareSigma = shareSigma;
            this.shareAlfa = shareAlfa;
            this.scaleSigma = scaleSigma;
            this.elitism = elitism;
            this.probCrossOver = probCrossOver;
            this.probMutability = probMutability;
            this.populationSize = populationSize;
            this.extinction = 0.0;
            this.newBorn = newBorn;
            this.zeros = zeros;
            generationNumber = 0;

            zerosSum = 0;
            for (int loop1 = 0; loop1 < zeros.Length; loop1++)
                if (zeros[loop1])
                    zerosSum++;

            population = new Chromozone[populationSize];
            for (int loop1 = 0; loop1 < populationSize; loop1++)
                population[loop1] = new Chromozone(rnd, zeros);

            theBestOne = new Chromozone(rnd, zeros);
            fitnessBestOne = 0.0;

            activeChromozomeID = 0;
            lastFitness = 0.0;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public int[][] GetChromozone()
        {
            return population[activeChromozomeID].GetGenes();
        }

        public void SetFitness(double fitness)
        {
            population[activeChromozomeID].SetFitness(fitness);
            lastFitness = fitness;
            activeChromozomeID++;

            if (activeChromozomeID >= population.Length)
            {
                NewPopulation();
                activeChromozomeID = 0;
                generationNumber++;
            }
        }

        private void ShareFactor()
        {
            for (int loop1 = 0; loop1 < population.Length; loop1++)
            {
                double share = 0.0;
                for (int loop2 = 0; loop2 < population.Length; loop2++)
                {
                    double dist = population[loop1].DistanceTo(population[loop2]);
                    if (dist < shareSigma)
                    {
                        share += 1.0 - Math.Pow((dist / shareSigma), shareAlfa);
                    }
                }
                population[loop1].SetFitness(population[loop1].GetFitness() / share);
            }


        }

        void ScaleFactor()
        {
            double sumFitness = population[0].GetFitness();
            double avgFitness;
            double maxFitness = population[0].GetFitness();
            double minFitness = population[0].GetFitness();
            double fitness;
            for (int loop1 = 1; loop1 < population.Length; loop1++)
            {
                fitness = population[loop1].GetFitness();
                sumFitness += fitness;
                if (fitness > maxFitness)
                    maxFitness = fitness;
                if (fitness < minFitness)
                    minFitness = fitness;
            }
            avgFitness = sumFitness / population.Length;
            double divideFactor = (maxFitness / avgFitness) / scaleSigma;
            double multiFactor = (avgFitness / minFitness) / scaleSigma;

            for (int loop1 = 0; loop1 < population.Length; loop1++)
            {
                if (population[loop1].GetFitness() > avgFitness)
                {
                    population[loop1].SetFitness(population[loop1].GetFitness() / divideFactor);
                }
                else
                {
                    population[loop1].SetFitness(population[loop1].GetFitness() * multiFactor);
                }
            }
        }

        private void KillTheWorsts(double trashhold)
        {
            int alive = 0;
            for (alive = 0; alive < population.Length; alive++)
            {
                if (population[alive].GetFitness() < trashhold)
                {
                    break;
                }
            }

            if (alive != population.Length)
            {
                Chromozone[] oldPopulation = new Chromozone[alive];
                for (int loop1 = 0; loop1 < oldPopulation.Length; loop1++)
                {
                    oldPopulation[loop1] = population[loop1];
                }
                population = oldPopulation;
            }
        }

        private void NewPopulation()
        {
            List<Chromozone> newPopulation = new List<Chromozone>();

            Array.Sort(population);
            Log();
            if (generationNumber % 3 == 0)
                PrintAllChromozones();

            if (population[0].GetFitness() > fitnessBestOne)
            {
                fitnessBestOne = population[0].GetFitness();
                theBestOne = population[0];
            }

            // At least the worst one wont produce itself
            if (population[population.Length - 1].GetFitness() > extinction)
                extinction = population[population.Length - 1].GetFitness() + 0.0001;

            for (int loop1 = 0; loop1 < elitism; loop1++)
            {
                newPopulation.Add(new Chromozone(population[loop1].GetGenes(), zeros));
            }

            KillTheWorsts(extinction);
            if(newBorn || population.Length < 2)
                AddFreshOnes(newPopulation);
            ScaleFactor();
            ShareFactor();

            double sumFitness = 0.0;
            double sumProb = 0.0;
            foreach (Chromozone ch in population)
            {
                sumFitness += ch.GetFitness();
            }
            foreach (Chromozone ch in population)
            {
                double prob = sumProb + ch.GetFitness() / sumFitness;
                ch.SetProbability(prob);
                sumProb += ch.GetFitness() / sumFitness;
            }

            int[][][] sons;

            while(newPopulation.Count < populationSize)
            {
                int[][] dad = Chromozone.CloneArray2D(Selection());
                int[][] mum = Chromozone.CloneArray2D(Selection());
                
                sons = CrossOver(dad, mum);

                for (int loop1 = 0; loop1 < sons.Length; loop1++)
                {
                    sons[loop1] = Mutation(sons[loop1]);
                    newPopulation.Add(new Chromozone(sons[loop1], zeros));
                }
            }

            population = newPopulation.ToArray();
            DifferPopulation();
        }

        private void DifferPopulation()
        {
            double tempMutability = probMutability;
            probMutability = 0.1;
            for (int loop1 = 0; loop1 < population.Length; loop1++)
            {
                for (int loop2 = 0; loop2 < population.Length; loop2++)
                {
                    if (loop1 != loop2 && population[loop1].DistanceTo(population[loop2]) == 0)
                    {
                        population[loop1] = new Chromozone(Mutation(population[loop1].GetGenes()), zeros);
                        loop2 = 0;
                    }
                }
            }
            probMutability = tempMutability;
        }

        private void AddFreshOnes(List<Chromozone> newPopulation)
        {
            int newOnes = 0;
            while (newPopulation.Count < populationSize && newOnes++ <= populationSize - population.Length)
            {
                newPopulation.Add(new Chromozone(rnd, zeros));
            }
        }

        private void Log()
        {
            double sum = 0.0;

            Chromozone best = null;
            double maxFitness = -0.1;
            double minFitness = population[0].GetFitness();
            string msg = "";
            foreach (Chromozone ch in population)
            {
                sum += ch.GetFitness();
                msg += ";" + String.Format("{0:0.00}", ch.GetFitness());
                if(ch.GetFitness() > maxFitness)
                {
                    best = ch;
                    maxFitness = ch.GetFitness();
                }

                if (ch.GetFitness() < minFitness)
                    minFitness = ch.GetFitness();
            }

            msg = "Fitness;" + String.Format("{0:0.00}", sum / populationSize) + ";" + String.Format("{0:0.00}", maxFitness) + ";" + String.Format("{0:0.00}", minFitness) + msg;
            Logger.Inst().Log("fitness.txt", msg);

            best.Log();          
        }

        private int[][] Mutation(int[][] entity)
        {
            for (int loop1 = 0; loop1 < entity.Length; loop1++)
            {
                if (entity[loop1][1] == 0)
                    continue;

                if (rnd.NextDouble() < probMutability)
                {
                    if (rnd.NextDouble() < 0.5)
                    {
                        entity[loop1][0] = rnd.Next(MAX_MAIN_COEF);
                    } else
                        for (int loop2 = 1; loop2 < entity[loop1].Length; loop2++)
                        {
                            entity[loop1][loop2] = rnd.Next(SUM_COEF) + 1;
                        }
                }
            }

            return entity;
        }

        private int CrossoverGenes(int a, int b, int range)
        {
            if (a == b)
                return a;

            double probSum = 0.0;
            double [] prob;
            prob = new double[range];
            double sigma = -Math.Abs(a-b)/2.0;
            double tempProb1;
            double tempProb2;
            for(int loop1 = 0; loop1 < prob.Length; loop1++)
            {
                int delta = a - loop1;
                tempProb1 = Math.Pow(Math.E, delta * delta / sigma);
                delta = b - loop1;
                tempProb2 = Math.Pow(Math.E, delta * delta / sigma);

                prob[loop1] = (tempProb1 > tempProb2) ? tempProb1 : tempProb2;

                probSum += prob[loop1];
            }

            double probPartSum = 0.0;
            Random rnd = new Random();
            double rndNumber = rnd.NextDouble();
            for(int loop1 = 0; loop1 < prob.Length; loop1++)
            {
                prob[loop1] = prob[loop1] / probSum + probPartSum;
                probPartSum = prob[loop1];
                if(probPartSum > rndNumber)
                    return loop1;
            }
            return range;
        }

        private int[][][] CrossOver(int[][] dad, int[][] mum)
        {
            int[][][] sons = new int[2][][];

            if (rnd.NextDouble() < probCrossOver)
            {
                for (int loop1 = 0; loop1 < sons.Length; loop1++)
                {
                    sons[loop1] = new int[dad.Length][];
                    for (int loop2 = 0; loop2 < sons[loop1].Length; loop2++)
                    {
                        sons[loop1][loop2] = new int[dad[loop2].Length];
                    }
                }
                
                int id1 = rnd.Next(zerosSum / 2 - 1) + 1;
                int id2 = rnd.Next(zerosSum / 2 - 1) + zerosSum / 2;

                int id = 0;
                for (int loop1 = 0; loop1 < dad.Length; loop1++)
                {
                    if (id < id1 || id >= id2)
                    {
                        sons[0][loop1] = dad[loop1];
                        sons[1][loop1] = mum[loop1];
                    } else {
                        sons[1][loop1] = dad[loop1];
                        sons[0][loop1] = mum[loop1];
                    }

                    if (dad[loop1][1] != 0)
                        id++;
                }

                    /*
                    for (int loop2 = 0; loop2 < sons[loop1].Length; loop2++)
                    {
                        if (rnd.NextDouble() < 0.2)
                        {
                            sons[loop1][loop2][0] = CrossoverGenes(dad[loop2][0], mum[loop2][0], MAX_MAIN_COEF);
                            for (int loop3 = 1; loop3 < sons[loop1][loop2].Length; loop3++)
                            {
                                sons[loop1][loop2][loop3] = CrossoverGenes(dad[loop2][loop3], mum[loop2][loop3], SUM_COEF);
                            }
                        }
                        else
                        {
                            for (int loop3 = 0; loop3 < sons[loop1][loop2].Length; loop3++)
                            {
                                sons[loop1][loop2][loop3] = (loop1 == 0) ? dad[loop2][loop3] : mum[loop2][loop3];
                            }
                        }
                          
                    } 
                }*/
            }
            else
            {
                sons[0] = mum;
                sons[1] = dad;
            }

            return sons;
        }

        private int[][] Selection()
        {
            double probRnd = rnd.NextDouble();

            int id = -1;

            for (int loop = 0; loop < population.Length; loop++)
            {
                double prob = population[loop].GetProbability();
                if (prob > probRnd)
                {
                    id = loop;
                    break;
                }
            }

            if (id < 0)
                id = population.Length - 1;

            return population[id].GetGenes();
        }

        internal void PrintAllChromozones()
        {
            foreach (Chromozone ch in population)
            {
                ch.Log("chrom" + generationNumber + ".txt");
            }
        }

        public int GetGenerationNumber()
        {
            return generationNumber;
        }

        public double GetLastFitness()
        {
            return lastFitness;
        }

        public int GetChromozonID()
        {
            return activeChromozomeID;
        }

        public int[][] GetBestOnePersonality()
        {
            return theBestOne.GetGenes();
        }

        public long GetTime()
        {
            return stopwatch.ElapsedMilliseconds;
        }

        public double GetExtinction()
        {
            return extinction;
        }
    }
}
