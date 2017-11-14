using BinPackingWPF.Algorithm;
using BinPackingWPF.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BinPackingWPF.Generator
{
    public class AlgorithmGenerator
    {
        private IList<Package> Packages;
        private Random random;
        private GeneticAlgorithm ga;
        private readonly double _binVolume;
        private readonly int _numGenerations;
        int populationSize = 100;

        public AlgorithmGenerator(IList<Package> packages, double binVolume, int numGenerations)
        {
            Packages = packages;
            random = new Random();
            _binVolume = binVolume;
            _numGenerations = numGenerations;
        }

        public IList<double> Generate()
        {
            ga = new GeneticAlgorithm(Packages, populationSize, random, _binVolume);

            if (_numGenerations == 1)
                return ga.FitnessOverTime;

            do
            {
                var task = Task.Factory.StartNew(() => ga.NewGeneration(crossoverNewDNA: true));
                task.Wait();
            }
            while (ga.Generation < _numGenerations);

            return ga.FitnessOverTime;
        }
    }
}
