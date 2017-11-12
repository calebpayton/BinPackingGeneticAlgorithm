using BinPackingWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinPackingWPF.Generator
{
    public class FleetGenerator
    {
        public Fleet Generate(IList<Package> packages, double binVolume)
        {
            var fleet = new Fleet();
            
            var bin = new Bin(binVolume);
            for (int i = 0; i < packages.Count; i++)
            {
                var occupiedVolume = bin.Packages.Count != 0 ? bin.Packages.Sum(p => p.Volume) : 0;

                if (bin.Volume - occupiedVolume >= packages[i].Volume)
                {
                    bin.Packages.Add(packages[i]);
                }
                else
                {
                    fleet.Bins.Add(bin);
                    bin = new Bin(binVolume);
                }
            }

            return fleet;
        }
    }
}
