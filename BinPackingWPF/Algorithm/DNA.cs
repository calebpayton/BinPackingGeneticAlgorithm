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
        public bool Grandparent { get; set; }
        public bool WisdomOfCrowds { get; private set; }

        private double binAcceptance = .99;
        private double wocAcceptance = .90;

        private Random _random;
        private readonly double _binVolume;
        private readonly FleetGenerator _fleetGenerator;

        public DNA(IList<Package> packages, Random random, double binVolume, Fleet fleet = null, bool shouldInitGenes = false)
        {
            Fleet = fleet;
            Packages = packages;
            Grandparent = false;
            WisdomOfCrowds = false;
            _random = random;
            _fleetGenerator = new FleetGenerator();
            _binVolume = binVolume;

            if (shouldInitGenes)
                Fleet = GetRandomFleet(Packages);

            if (Fleet != null)
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

        public DNA Crossover()
        {
            List<Bin> optimalBins = null;
            if (Fleet.Bins.Where(b => b.Fitness > binAcceptance).Any())
                optimalBins = Fleet.Bins.Where(b => b.Fitness > binAcceptance).ToList();
            else
                optimalBins = new List<Bin> { Fleet.Bins.First() };

            var optimalPackages = optimalBins.SelectMany(b => b.Packages).ToList();
            var otherPackages = Packages.Where(p => !optimalPackages.Contains(p)).ToList();

            var randomizedPackages = Shuffle(otherPackages);

            optimalPackages.AddRange(randomizedPackages);

            var fleet = _fleetGenerator.Generate(optimalPackages, _binVolume);

            var child = new DNA(Packages, _random, _binVolume, fleet);

            return child;
        }

        public void CrossoverWoC(IList<DNA> grandparents)
        {
            List<Bin> optimalBins = null;
            if (grandparents.First().Fleet.Bins.Where(b => b.Fitness > wocAcceptance).Any())
                optimalBins = grandparents.First().Fleet.Bins.Where(b => b.Fitness > wocAcceptance).ToList();
            else
                optimalBins = new List<Bin> { grandparents.First().Fleet.Bins.First() };

            var optimalPackages = optimalBins.SelectMany(b => b.Packages).ToList();
            var otherPackages = Packages.Where(p => !optimalPackages.Contains(p)).ToList();

            var fleet = _fleetGenerator.Generate(optimalPackages, _binVolume);
            
            while (otherPackages.Sum(p => p.Volume) > _binVolume * 3)
            {
                var randomizedPackages = Shuffle(otherPackages);
                var subFleet = _fleetGenerator.Generate(randomizedPackages, _binVolume);
                if (subFleet.Bins.Where(b => b.Fitness > binAcceptance).Any())
                {
                    fleet.Bins.AddRange(subFleet.Bins.Where(b => b.Fitness > binAcceptance));
                    var packagesToRemove = subFleet.Bins.Where(b => b.Fitness > binAcceptance).SelectMany(b => b.Packages).ToList();
                    packagesToRemove.ForEach(p =>
                    {
                        otherPackages.Remove(p);
                    });
                }
            }

            if (otherPackages.Any())
            {
                var finalSubFleet = _fleetGenerator.Generate(otherPackages, _binVolume);
                fleet.Bins.AddRange(finalSubFleet.Bins);
            }

            this.Fleet = fleet;
            this.Fitness = CalculateFitness();
            this.WisdomOfCrowds = true;
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
