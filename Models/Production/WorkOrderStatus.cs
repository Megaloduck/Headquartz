using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Production
{
    public enum WorkOrderStatus
    {
        Scheduled,
        WaitingMaterials,
        InProgress,
        Completed,
        Cancelled
    }
}
