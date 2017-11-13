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
        int populationSize = 100;

        public AlgorithmGenerator(IList<Package> packages, double binVolume)
        {
            Packages = packages;
            random = new Random();
            _fleetGenerator = new FleetGenerator();
            _binVolume = binVolume;
        }

        public Fleet Generate()
        {
            ga = new GeneticAlgorithm(Packages, populationSize, random, _binVolume);

            do
            {
                ga.NewGeneration(crossoverNewDNA: true);
            }
            while (ga.Generation < 10);

            return ga.BestFleet;
        }
    }
}
