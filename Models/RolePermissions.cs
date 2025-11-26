using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class RolePermissions
    {
        public string Name { get; set; } = "CEO";
        public bool CanSeeFinance { get; set; }
        public bool CanManageHR { get; set; }
        public bool CanSeeMarket { get; set; }
        public bool CanManageWarehouse { get; set; }
        public bool CanManageReports { get; set; }
    }
}
