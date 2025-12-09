using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class QuickPickItem
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string Location { get; set; }
        public string OrderNumber { get; set; }
        public int Quantity { get; set; }
        public string Priority { get; set; }
        public string PriorityColor { get; set; }
    }
}
