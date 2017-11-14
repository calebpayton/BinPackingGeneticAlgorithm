using BinPackingWPF.Model;
using System;
using System.Collections.Generic;

namespace BinPackingWPF.Generator
{
    public class PackagesGenerator
    {
        double packageVolumeFactor = .25;

        public IList<Package> GeneratePackages(int numPackages, double binVolume)
        {
            var packages = new List<Package>();
            
            var random = new Random();
            for (int i = 0; i < numPackages; i++)
            {
                packages.Add(new Package
                {
                    Volume = random.NextDouble() * binVolume * packageVolumeFactor,
                    Id = i + 1
                });
            }

            return packages;
        }
    }
}