using System.Collections.Generic;

namespace BinPackingWPF.Model
{
    public class Fleet
    {
        public List<Bin> Bins { get; set; }

        public Fleet()
        {
            Bins = new List<Bin>();
        }
    }
}