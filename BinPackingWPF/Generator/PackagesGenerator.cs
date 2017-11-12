using BinPackingWPF.Model;
using System;
using System.Collections.Generic;

namespace BinPackingWPF.Generator
{
    public class PackagesGenerator
    {
        public IList<Package> GeneratePackages(int numPackages, double maxVolume)
        {
            var packages = new List<Package>();
            
            var random = new Random();
            for (int i = 0; i < numPackages; i++)
            {
                packages.Add(new Package
                {
                    Volume = random.NextDouble() * maxVolume
                });
            }

            return packages;
        }
    }
}