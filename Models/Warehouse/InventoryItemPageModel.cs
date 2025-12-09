using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class InventoryItemPageModel
    {
        public string Id { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string CategoryColor { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public decimal TotalValue { get; set; }
        public double StockPercentage { get; set; }
        public string StockColor { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
    }
}
