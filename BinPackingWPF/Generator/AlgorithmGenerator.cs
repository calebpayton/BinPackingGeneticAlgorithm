using BinPackingWPF.Algorithm;
using BinPackingWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinPackingWPF.Generator
{
    public class AlgorithmGenerator
    {
        private IList<Package> Packages;
        private Random random;
        private GeneticAlgorithm ga;
        private readonly FleetGenerator _fleetGenerator;
        private readonly double _binVolume;
        int populationSize = 10;

        public AlgorithmGenerator(IList<Package> packages, double binVolume)
        {
            Packages = packages;
            random = new Random();
            _fleetGenerator = new FleetGenerator();
            _binVolume = binVolume;
        }

        public void Generate()
        {
            ga = new GeneticAlgorithm(Packages, populationSize, random, GetRandomFleet, FitnessFunction, 1);
            ga.NewGeneration(crossoverNewDNA: true);
        }

        public IList<Fleet> GetStartingGeneration()
        {
            var fleets = new List<Fleet>();
            for (int i = 0; i < 11; i++)
            {
                fleets.Add(_fleetGenerator.Generate(Packages, _binVolume));
            }

            return fleets;
        }

        public Fleet GetRandomFleet(IList<Package> list)
        {
            var newPackageAssortment = Shuffle(list);

            return _fleetGenerator.Generate(newPackageAssortment, _binVolume);
        }

        private float FitnessFunction(Fleet fleet)
        {
            float score = 0;

            foreach (var bin in fleet.Bins)
            {
                if (bin.Packages.Sum(p => p.Volume) / bin.Volume == .9) //if packages in bin occupy 90% of bin volume
                {
                    score += 1;
                }
            }

            return score /= fleet.Bins.Count;
        }

        public IList<Package> Shuffle(IList<Package> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
