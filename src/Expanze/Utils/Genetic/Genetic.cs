using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.Utils.Genetic
{
    class Genetic
    {
        Chromozone[] population;
        int activeChromozomeID;
        int populationSize;
        double probCrossOver;
        double probMutability;
        int generationNumber;
        Random rnd;

        public Genetic(int populationSize, double probCrossOver, double probMutability)
        {
            rnd = new Random();
            this.probCrossOver = probCrossOver;
            this.probMutability = probMutability;
            this.populationSize = populationSize;
            generationNumber = 0;

            population = new Chromozone[populationSize];
            for (int loop1 = 0; loop1 < populationSize; loop1++)
                population[loop1] = new Chromozone(rnd);

            activeChromozomeID = 0;
        }

        public int[][] GetChromozone()
        {
            return population[activeChromozomeID].GetGenes();
        }

        public void SetFitness(double fitness)
        {
            population[activeChromozomeID].SetFitness(fitness);
            activeChromozomeID++;

            if (activeChromozomeID >= population.Length)
            {
                NewPopulation();
                activeChromozomeID = 0;
                generationNumber++;
            }
        }

        private void NewPopulation()
        {
            Log();
            List<Chromozone> newPopulation = new List<Chromozone>();

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
                int[][] dad = Selection();
                int[][] mum = Selection();
                
                sons = CrossOver(dad, mum);

                for (int loop1 = 0; loop1 < 2; loop1++)
                {
                    sons[loop1] = Mutation(sons[loop1]);
                    newPopulation.Add(new Chromozone(sons[loop1]));
                }
            }

            population = newPopulation.ToArray();
        }

        private void Log()
        {
            double sum = 0.0;

            Chromozone best = null;
            double maxFitness = -0.1;
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
            }

            msg = "Fitness;" + String.Format("{0:0.00}", sum / populationSize) + msg;
            Logger.Inst().Log("fitness.txt", msg);

            best.Log();          
        }

        private int[][] Mutation(int[][] entity)
        {
            for (int loop1 = 0; loop1 < entity.Length; loop1++)
            {
                for (int loop2 = 0; loop2 < entity[loop1].Length; loop2++)
                {
                    int gene = entity[loop1][loop2];
                    if (rnd.NextDouble() < probMutability)
                    {
                        gene = rnd.Next(1000);
                    }
                    entity[loop1][loop2] = gene;
                }
            }

            return entity;
        }

        private int[][][] CrossOver(int[][] dad, int[][] mum)
        {
            int[][][] sons = new int[2][][];
            for (int loop1 = 0; loop1 < sons.Length; loop1++)
            {
                sons[loop1] = new int[dad.Length][];
                for (int loop2 = 0; loop2 < sons[loop1].Length; loop2++)
                {
                    sons[loop1][loop2] = new int[dad[loop2].Length];
                }
            }

            if (rnd.NextDouble() < probCrossOver)
            {
                int breakID1 = 1 + rnd.Next() % (dad.Length - 1);
                int breakID2;
                do
                {
                    breakID2 = 1 + rnd.Next() % (dad.Length - 1);
                } while (breakID2 == breakID1);

                int temp;
                if (breakID2 < breakID1)
                {
                    temp = breakID1;
                    breakID1 = breakID2;
                    breakID2 = temp;
                }

                for (int loop1 = 0; loop1 < dad.Length; loop1++)
                {
                    if (loop1 < breakID1 || loop1 > breakID2)
                    {
                        sons[0][loop1] = mum[loop1];
                        sons[1][loop1] = dad[loop1];
                    }
                    else
                    {
                        sons[0][loop1] = dad[loop1];
                        sons[1][loop1] = mum[loop1];
                    }
                }
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

        public int GetChromozonID()
        {
            return activeChromozomeID;
        }
    }
}
