using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.PageModels.Warehouse
{
    public class ShippingScheduleItem
    {
        public string Id { get; set; }
        public string TimeSlot { get; set; }
        public string Carrier { get; set; }
        public int OrdersCount { get; set; }
        public string Destination { get; set; }
        public int PackageCount { get; set; }
        public int TotalWeight { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
    }
}
