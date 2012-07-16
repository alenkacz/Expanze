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

        Chromozone theBestOne; // the best chromozome in all populations
        double fitnessBestOne; // its fitness
        
        double lastFitness;    // fitness to show on screen
        Stopwatch stopwatch;   // time of evolution

        int activeChromozomeID; // this one is now playing
        int populationSize;
        int elitism;
        double probCrossOver; // probability of cross over
        double probMutability; // probability of mutation
        int generationNumber;
        double shareSigma;
        double shareAlfa;
        double scaleSigma;
        double extinction;
        bool newBorn;
        Random rnd;
        bool[] zeros;

        int zerosSum; // how many inactive genes are in chromozome?

        /// <summary>
        /// Creates new random population and sets parameters of evolution
        /// </summary>
        /// <param name="populationSize">Population size</param>
        /// <param name="probCrossOver">Probability of crossover</param>
        /// <param name="probMutability">Probability of mutation</param>
        /// <param name="elitism">How many best ones will survive?</param>
        /// <param name="shareSigma">Distance, two chromozome with distance below that parameter are similiar</param>
        /// <param name="shareAlfa">Sharing constant</param>
        /// <param name="scaleSigma">Scaling constant, best fitnuss would be scaleSigma times greater than average population fitness</param>
        /// <param name="newBorn">True if extincted ones should be replace by random ones</param>
        /// <param name="zeros">Which genes arent active in this evolution? (ex. building fort is banned in a scenario)</param>
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
            
            // generate random start population
            for (int loop1 = 0; loop1 < populationSize; loop1++)
                population[loop1] = new Chromozone(rnd, zeros);

            theBestOne = population[0];
            fitnessBestOne = 0.0;

            activeChromozomeID = 0;
            lastFitness = 0.0;

            // start time of evolving
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Chromozome which has no fitness now</returns>
        public int[][] GetChromozone()
        {
            return population[activeChromozomeID].GetGenes();
        }

        /// <summary>
        /// Sets fitness of active chromozome
        /// If this chromozome was last one without fitness, 
        /// we evolve new generation of population
        /// </summary>
        /// <param name="fitness">New fitness of active chromozome</param>
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

        private void NewPopulation()
        {
            // create empty list of new population
            List<Chromozone> newPopulation = new List<Chromozone>();

            // sort chromozomes according their fitness
            Array.Sort(population);
            Log();
            if (generationNumber % 3 == 0) // in each third generation, print all chromozomes in text file
                PrintAllChromozones();

            // replace best one in all populations
            if (population[0].GetFitness() > fitnessBestOne)
            {
                fitnessBestOne = population[0].GetFitness();
                theBestOne = population[0];
            }

            // At least the worst one wont produce itself
            // Adds something to exctinction
            for (int loop1 = population.Length - 1; loop1 >= population.Length / 3; loop1--)
            {
                if (population[loop1].GetFitness() > extinction &&
                    population[loop1].GetFitness() != population[population.Length / 3 - 1].GetFitness())
                {
                    extinction = population[loop1].GetFitness();
                    break;
                }
            }

            // apply elitism
            for (int loop1 = 0; loop1 < elitism; loop1++)
            {
                newPopulation.Add(new Chromozone(population[loop1].GetGenes(), zeros));
            }

            // kill all chromozome with fitness less than extinction value
            KillTheWorsts(extinction);
            
            // replace extincted ones by new random ones
            if (newBorn || population.Length < 2)
                AddFreshOnes(newPopulation);

            // linear scaling of fitness
            ScaleFactor();
            // change fitnesses according share value
            ShareFactor();

            // prepare roulete wheel for selectioin
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
            // end of preparing roulete wheel

            int[][][] sons;
            while (newPopulation.Count < populationSize)
            {
                // clone selected mum and dad
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

            // when there are exactly same chromozome in new population, mutate the copyiest until thera arent two same chromozomes
            DifferPopulation();
        }

        /// <summary>
        /// Changes fitnesses of chromozome according sharing factor with other ones
        /// </summary>
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

        /// <summary>
        /// Linear scaling of fitness. The best one should be scaleSigma times better than average fitness of population.
        /// </summary>
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
                // better than average
                if (population[loop1].GetFitness() > avgFitness)
                {
                    population[loop1].SetFitness(population[loop1].GetFitness() / divideFactor);
                }
                else // worse than average
                {
                    population[loop1].SetFitness(population[loop1].GetFitness() * multiFactor);
                }
            }
        }

        private void KillTheWorsts(double trashhold)
        {
            int alive = 0;
            // how many will survive?
            for (alive = 0; alive < population.Length; alive++)
            {
                if (population[alive].GetFitness() < trashhold)
                {
                    break;
                }
            }

            // copy survived chromozome to new array and replace with it the old one
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

        /// <summary>
        /// For the last step of evolution new generation.
        /// If there are 2 exactly same chromozomes, mutate one of them.
        /// </summary>
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

            msg = "Fitness;" + String.Format("{0:0.00}", sum / population.Length) + ";" + String.Format("{0:0.00}", maxFitness) + ";" + String.Format("{0:0.00}", minFitness) + ";" + String.Format("{0:0.00}", extinction) + msg;
            Logger.Inst().Log("fitness.txt", msg);

            best.Log();          
        }

        /// <summary>
        /// Mutates entity with probMutability probability
        /// </summary>
        /// <param name="entity">Who will be mutate</param>
        /// <returns>Mutated one</returns>
        private int[][] Mutation(int[][] entity)
        {
            for (int loop1 = 0; loop1 < entity.Length; loop1++)
            {
                if (entity[loop1][1] == 0)
                    continue;

                if (rnd.NextDouble() < probMutability)
                {
                    double probKind = rnd.NextDouble();

                    if (rnd.NextDouble() < 0.33) // mutate big coeficient
                    {
                        entity[loop1][0] = rnd.Next(MAX_MAIN_COEF);
                    }
                    else if (rnd.NextDouble() < 0.66) // mutate small coeficient
                    {
                        for (int loop2 = 1; loop2 < entity[loop1].Length; loop2++)
                        {
                            entity[loop1][loop2] = rnd.Next(SUM_COEF) + 1;
                        }
                    }
                    else // change maxGene between two sets of genes
                    {
                        int[] tempMaxGene = entity[loop1 % 11];
                        entity[loop1 % 11] = entity[loop1 % 11 + 11];
                        entity[loop1 % 11 + 11] = tempMaxGene;
                    }
                }
            }

            return entity;
        }

        /// <summary>
        /// Gaussian crossover of two numbers a and b. Returned number will be in most cases near to one of two numbers.
        /// </summary>
        /// <param name="a">Crossovered number</param>
        /// <param name="b">Crossovered number</param>
        /// <param name="range">Max value of gene</param>
        /// <returns>Crossovered number</returns>
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

        /// <summary>
        /// Crossover parents with probability probCrossOver
        /// </summary>
        /// <param name="dad">Dad</param>
        /// <param name="mum">Mum</param>
        /// <returns>2 sons</returns>
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

                int id = 0;
                for (int loop1 = 0; loop1 < dad.Length; loop1++)
                {
                    if (rnd.NextDouble() < 0.5)
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
            }
            else
            {
                sons[0] = mum;
                sons[1] = dad;
            }

            return sons;
        }

        /// <summary>
        /// Selection by rulette wheel
        /// </summary>
        /// <returns>Selected chromozome</returns>
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

        /// <summary>
        /// Parse personality from string
        /// </summary>
        /// <param name="personalityText">String two parse</param>
        /// <returns>Chromozome</returns>
        public static int[][] ParsePersonality(string personalityText)
        {
            int[][] playerPersonality = new int[22][];
            for (int loop1 = 0; loop1 < 2; loop1++)
            {
                playerPersonality[0 + loop1 * 11] = new int[4];
                playerPersonality[1 + loop1 * 11] = new int[2];
                playerPersonality[2 + loop1 * 11] = new int[4];
                playerPersonality[3 + loop1 * 11] = new int[5];
                playerPersonality[4 + loop1 * 11] = new int[2];
                playerPersonality[5 + loop1 * 11] = new int[6];
                playerPersonality[6 + loop1 * 11] = new int[6];
                playerPersonality[7 + loop1 * 11] = new int[2];
                playerPersonality[8 + loop1 * 11] = new int[2];
                playerPersonality[9 + loop1 * 11] = new int[2];
                playerPersonality[10 + loop1 * 11] = new int[2];
            }

            String[] koef = personalityText.Split(";".ToCharArray());
            int koefID = 0;
            for (int loop1 = 0; loop1 < playerPersonality.Length; loop1++)
            {
                for (int loop2 = 0; loop2 < playerPersonality[loop1].Length; loop2++)
                {
                    while (koefID < koef.Length - 1 && koef[koefID] == "")
                        koefID++;

                    playerPersonality[loop1][loop2] = Convert.ToInt32(koef[koefID++]);
                }
            }

            return playerPersonality;
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
