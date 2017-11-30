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
        public IList<double> FitnessOverTime { get; private set; }
        public int Generation { get; private set; }
        public double BestFitness { get; private set; }
        public double WoCFitness { get; private set; }
        public Fleet BestFleet { get; private set; }
        public bool FinalGeneration { get; set; }
        public float MutationRate;        
        private Random _random;
        private readonly double _binVolume;

        public GeneticAlgorithm(IList<Package> packages, int populationSize, Random random, double binVolume, float mutationRate = 0.01f)
        {
            FitnessOverTime = new List<double>();
            Packages = packages;
            Generation = 1;
            MutationRate = mutationRate;
            Population = new List<DNA>(populationSize);
            FinalGeneration = false;
            _random = random;
            _binVolume = binVolume;

            for (int i = 0; i < populationSize; i++)
            {
                var fleet = new Fleet();
                Population.Add(new DNA(Packages, _random, _binVolume, shouldInitGenes: true));
            }

            Population.Sort(CompareDNA);
            BestFitness = Population.First().Fitness;
            BestFleet = Population.First().Fleet;

            FitnessOverTime.Add(BestFitness);
        }

        public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
        {
            var newPopulation = new List<DNA>(Population.Count);

            for (int i = 0; i < Population.Count; i++)
            {
                if (crossoverNewDNA)
                {
                    var parent = Population.First();
                    var child = parent.Crossover();

                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new DNA(Packages, _random, _binVolume, shouldInitGenes: true));
                }
            }

            newPopulation.Sort(CompareDNA);
            if (Population.First().Fitness > newPopulation.First().Fitness)
                Population.First().Grandparent = true;
            else
                newPopulation.First().Grandparent = true;

            newPopulation.AddRange(Population.Where(p => p.Grandparent == true));
            newPopulation.Sort(CompareDNA);

            for (int i = Population.Count - 1; i < newPopulation.Count; i++)
            {
                newPopulation.RemoveAt(i);
            }

            if (FinalGeneration == true)
            {
                var wocChild = new DNA(Packages, _random, _binVolume);
                wocChild.CrossoverWoC(Population.Where(p => p.Grandparent == true).ToList());
                WoCFitness = wocChild.Fitness;
            }

            Population = newPopulation;
            Population.Sort(CompareDNA);

            BestFitness = Population.First().Fitness;
            BestFleet = Population.First().Fleet;

            FitnessOverTime.Add(BestFitness);

            Generation++;
        }

        public int CompareDNA(DNA a, DNA b)
        {
            if (a.Fitness > b.Fitness)
                return -1;
            else if (a.Fitness < b.Fitness)
                return 1;
            else
                return 0;
        }

        private DNA ChooseParent()
        {
            double randomNumber = _random.NextDouble() * (Population.Count - 1);

            return Population[Convert.ToInt32(randomNumber)];
        }
    }
}
