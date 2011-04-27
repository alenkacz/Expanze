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
        Random rnd;

        public Genetic(int populationSize, int chromozomeSize)
        {
            this.populationSize = populationSize;

            population = new Chromozone[populationSize];
            for (int loop1 = 0; loop1 < populationSize; loop1++)
                population[loop1] = new Chromozone(chromozomeSize);

            activeChromozomeID = 0;
            rnd = new Random();
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
            Chromozone[] winners = Selection(population);
            population = CrossOver(winners);
            Mutation(population);
        }

        private void Log()
        {
            double sum = 0.0;
            foreach (Chromozone ch in population)
            {
                sum += ch.GetFitness();
            }

            Logger.Inst().Log("fitness.txt", "Fitness > " + sum / populationSize);
        }

        double mutationProb = 0.01;

        private void Mutation(Chromozone[] generation)
        {
            for (int loop1 = 0; loop1 < generation.Length; loop1++)
            {
                if (rnd.NextDouble() < mutationProb)
                {
                    double[] origin = generation[loop1].GetGenes();
                    double[] mutated = new double[origin.Length];

                    for (int loop2 = 0; loop2 < origin.Length; loop2++)
                    {
                        double gene = origin[loop2] + (rnd.NextDouble() - 0.5) / 20.0;
                        if (gene > 1.0) gene = 1.0;
                        if (gene < 0.0) gene = 0.0;
                        mutated[loop2] = gene;
                    }

                    generation[loop1] = new Chromozone(mutated);
                }
            }
        }

        private Chromozone[] CrossOver(Chromozone[] winners)
        {
            Random rnd = new Random();
            Chromozone[] generation = new Chromozone[populationSize];

            for (int loop1 = 0; loop1 < populationSize; loop1++)
            {
                int id1 = rnd.Next() % winners.Length;
                int id2 = rnd.Next() % winners.Length;

                generation[loop1] = CrossOver(winners[id1], winners[id2]);
            }

            return generation;
        }

        private Chromozone CrossOver(Chromozone chromozone1, Chromozone chromozone2)
        {
            int chromLength = chromozone1.GetGenes().Length;
            double [] newChromozone = new double[chromLength];

            double fitness1 = chromozone1.GetFitness();
            double fitness2 = chromozone2.GetFitness();
            double prob1 = fitness1 / (fitness1 + fitness2);
            double prob2 = fitness2 / (fitness1 + fitness2);
            double[] genes1 = chromozone1.GetGenes();
            double[] genes2 = chromozone2.GetGenes();

            for (int loop1 = 0; loop1 < chromLength; loop1++)
            {
                newChromozone[loop1] = (rnd.NextDouble() < prob1) ? genes1[loop1] : genes2[loop1];
            }

            return new Chromozone(newChromozone);
        }

        private Chromozone[] Selection(Chromozone[] generation)
        {
            Random rnd = new Random();

            int elits = populationSize / 2;
            int others = populationSize / 4;
            Chromozone[] winners = new Chromozone[elits + others];

            Array.Sort(generation);

            generation[0].Log();

            for (int loop1 = 0; loop1 < elits; loop1++)
            {
                winners[loop1] = generation[loop1];
            }

            for (int loop1 = elits; loop1 < elits + others; loop1++)
            {
                int id = (rnd.Next() % (populationSize - elits)) + elits;
                winners[loop1] = generation[id];
            }

            return winners;
        }
    }
}
