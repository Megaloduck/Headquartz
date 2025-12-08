using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class InventoryItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastRestocked { get; set; }
        public int ReorderLevel { get; set; } = 50;
        public int ReorderQuantity { get; set; } = 100;
    }
}
