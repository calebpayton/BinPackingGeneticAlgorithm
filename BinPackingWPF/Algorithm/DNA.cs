using BinPackingWPF.Generator;
using BinPackingWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinPackingWPF.Algorithm
{
    public class DNA
    {
        public Fleet Fleet { get; private set; }
        public IList<Package> Packages { get; private set; }
        public double Fitness { get; private set; }

        private Random _random;
        private readonly double _binVolume;
        private readonly FleetGenerator _fleetGenerator;

        public DNA(IList<Package> packages, Random random, double binVolume, Fleet fleet = null, bool shouldInitGenes = true)
        {
            Fleet = fleet;
            Packages = packages;
            _random = random;
            _fleetGenerator = new FleetGenerator();
            _binVolume = binVolume;

            if (shouldInitGenes)
            {
                Fleet = GetRandomFleet(Packages);
            }

            Fitness = CalculateFitness();
        }

        public double CalculateFitness()
        {
            var fitness = Fleet.Bins.SelectMany(b => b.Packages).Sum(p => p.Volume) / Fleet.Bins.Sum(b => b.Volume);

            return fitness;
        }

        public Fleet GetRandomFleet(IList<Package> list)
        {
            var newPackageAssortment = Shuffle(list);

            return _fleetGenerator.Generate(newPackageAssortment, _binVolume);
        }

        public IList<Package> Shuffle(IList<Package> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                var k = _random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public DNA Crossover(DNA otherParent)
        {
            List<Bin> optimalBins = null;
            if (Fleet.Bins.Where(b => b.Fitness > .99).Any())
                optimalBins = Fleet.Bins.Where(b => b.Fitness > .99).ToList();
            else
                optimalBins = new List<Bin> { Fleet.Bins.First() };

            var optimalPackages = optimalBins.SelectMany(b => b.Packages).ToList();
            var otherPackages = otherParent.Fleet.Bins.SelectMany(b => b.Packages).Where(p => !optimalPackages.Contains(p)).ToList();

            var randomizedPackages = Shuffle(otherPackages);

            optimalPackages.AddRange(randomizedPackages);

            var fleet = _fleetGenerator.Generate(optimalPackages, _binVolume);

            var child = new DNA(Packages, _random, _binVolume, fleet, false);

            return child;
        }

        public void Mutate(float mutationRate)
        {
            if (_random.NextDouble() < mutationRate)
            {
                Fleet = GetRandomFleet(Packages);
                Fitness = CalculateFitness();
            }
        }
    }
}
