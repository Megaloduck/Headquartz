using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class PendingReceiptItem
    {
        public string Id { get; set; }
        public string ReceiptNumber { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public string PONumber { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpectedDate { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
    }
}
