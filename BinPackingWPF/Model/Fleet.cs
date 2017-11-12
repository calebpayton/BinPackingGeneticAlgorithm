using System.Collections.Generic;

namespace BinPackingWPF.Model
{
    public class Fleet
    {
        public IList<Bin> Bins { get; set; }

        public Fleet()
        {
            Bins = new List<Bin>();
        }
    }
}