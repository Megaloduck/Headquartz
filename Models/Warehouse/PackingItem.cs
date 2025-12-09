using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class PackingItem
    {
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public int ItemCount { get; set; }
        public string Picker { get; set; }
        public DateTime PickTime { get; set; }
    }
}
