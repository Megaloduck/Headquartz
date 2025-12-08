using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Production
{
    public class WorkOrder
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public WorkOrderStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public Dictionary<string, int> RequiredMaterials { get; set; }
    }
}
