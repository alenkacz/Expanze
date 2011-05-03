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
        Random rnd;

        public Genetic(int populationSize, int chromozomeSize, double probCrossOver, double probMutability)
        {
            rnd = new Random();
            this.probCrossOver = probCrossOver;
            this.probMutability = probMutability;
            this.populationSize = populationSize;

            population = new Chromozone[populationSize];
            for (int loop1 = 0; loop1 < populationSize; loop1++)
                population[loop1] = new Chromozone(chromozomeSize, rnd);

            activeChromozomeID = 0;
        }

        public double[] GetChromozone()
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

            double[][] sons;

            while(newPopulation.Count < populationSize)
            {
                double[] dad = Selection();
                double[] mum = Selection();
                
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
            double maxFitness = 0.0;
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

        private double[] Mutation(double[] entity)
        {
            for (int loop = 0; loop < entity.Length; loop++)
            {
                double gene = entity[loop];
                if (rnd.NextDouble() < probMutability)
                {
                    gene += (rnd.NextDouble() - 0.5) / 20.0;
                    if (gene > 1.0) gene = 1.0;
                    if (gene < 0.0) gene = 0.0;
                }
                entity[loop] = gene;
            }

            return entity;
        }

        private double[][] CrossOver(double[] dad, double[] mum)
        {
            double[][] sons = new double[2][];
            for (int loop1 = 0; loop1 < sons.Length; loop1++)
                sons[loop1] = new double[dad.Length];

            if (rnd.NextDouble() < probCrossOver)
            {
                int breakID = rnd.Next() % dad.Length;
                for (int loop1 = 0; loop1 < dad.Length; loop1++)
                {
                    if (breakID < loop1 + 1)
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

        private double[] Selection()
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
    }
}
