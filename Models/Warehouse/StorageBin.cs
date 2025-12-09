using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class StorageBin
    {
        public string Id { get; set; }
        public string BinCode { get; set; }
        public string Zone { get; set; }
        public double Occupancy { get; set; }
        public int ProductCount { get; set; }
        public string StatusColor { get; set; }
    }
}
