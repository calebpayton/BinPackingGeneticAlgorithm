using BinPackingWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinPackingWPF.Algorithm
{
    public class GeneticAlgorithm
    {
        public List<DNA> Population { get; private set; }
        public IList<Package> Packages { get; private set; }
        public int Generation { get; private set; }
        public float BestFitness { get; private set; }
        public Fleet BestFleet { get; private set; }

        public float MutationRate;

        private List<DNA> newPopulation;
        private Random random;
        private float fitnessSum;
        private Func<IList<Package>, Fleet> getRandomFleet;
        private Func<Fleet, float> fitnessFunction;

        public GeneticAlgorithm(IList<Package> packages, int populationSize, Random random, Func<IList<Package>, Fleet> getRandomFleet, Func<Fleet, float> fitnessFunction, float mutationRate = 0.01f)
        {
            Packages = packages;
            Generation = 1;
            MutationRate = mutationRate;
            Population = new List<DNA>(populationSize);
            newPopulation = new List<DNA>(populationSize);
            this.random = random;
            this.getRandomFleet = getRandomFleet;
            this.fitnessFunction = fitnessFunction;

            BestFleet = new Fleet();

            for (int i = 0; i < populationSize; i++)
            {
                var fleet = new Fleet();
                Population.Add(new DNA(Packages, random, getRandomFleet, fitnessFunction, fleet, shouldInitGenes: true));
            }
        }

        public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
        {
            int finalCount = Population.Count + numNewDNA;

            if (finalCount <= 0)
            {
                return;
            }

            if (Population.Count > 0)
            {
                CalculateFitness();
                Population.Sort(CompareDNA);
            }
            newPopulation.Clear();

            for (int i = 0; i < Population.Count; i++)
            {
                if (i < Population.Count)
                {
                    newPopulation.Add(Population[i]);
                }
                else if (i < Population.Count || crossoverNewDNA)
                {
                    DNA parent1 = ChooseParent();
                    DNA parent2 = ChooseParent();

                    DNA child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new DNA(Packages, random, getRandomFleet, fitnessFunction, shouldInitGenes: true));
                }
            }

            List<DNA> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;

            Generation++;
        }

        public int CompareDNA(DNA a, DNA b)
        {
            if (a.Fitness > b.Fitness)
            {
                return -1;
            }
            else if (a.Fitness < b.Fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void CalculateFitness()
        {
            fitnessSum = 0;
            DNA best = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalculateFitness(Population[i].Fleet);

                if (Population[i].Fitness > best.Fitness)
                {
                    best = Population[i];
                }
            }

            BestFitness = best.Fitness;
            BestFleet = best.Fleet;
        }

        private DNA ChooseParent()
        {
            double randomNumber = random.NextDouble() * fitnessSum;

            for (int i = 0; i < Population.Count; i++)
            {
                if (randomNumber < Population[i].Fitness)
                {
                    return Population[i];
                }

                randomNumber -= Population[i].Fitness;
            }

            return null;
        }
    }
}
