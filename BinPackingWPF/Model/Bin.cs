using System.Collections.Generic;

namespace BinPackingWPF.Model
{
    public class Bin
    {
        public double Volume { get; set; }
        public IList<Package> Packages { get; set; }
        public double Fitness { get; set; }

        public Bin(double volume)
        {
            Packages = new List<Package>();
            Volume = volume;
        }
    }
}