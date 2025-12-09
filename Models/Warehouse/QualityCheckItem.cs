using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class QualityCheckItem
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Inspector { get; set; }
    }
}
