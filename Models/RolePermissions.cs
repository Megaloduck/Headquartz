using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class RolePermissions
    {
        public PlayerRole Role { get; set; }
        public bool CanSeeFinance { get; set; }
        public bool CanManageHR { get; set; }
        public bool CanSeeMarket { get; set; }
        public bool CanManageInventory { get; set; }
    }
}
