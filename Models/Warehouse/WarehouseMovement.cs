using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class WarehouseMovement
    {
        public string Id { get; set; }
        public string MovementType { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public DateTime MovementTime { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public string TypeColor { get; set; }
        public string Icon { get; set; }
        public string QuantityColor { get; set; }
    }
}
