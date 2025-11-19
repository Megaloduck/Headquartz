using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Services
{
    public class RoleService
    {
        public PlayerRole CurrentRole { get; private set; }

        public void SetRole(PlayerRole role)
        {
            CurrentRole = role;
        }
    }

    public class PlayerRole
    {
        public string Name { get; set; }
        public bool CanSeeFinance { get; set; }
        public bool CanManageHR { get; set; }
        public bool CanSeeMarket { get; set; }
        public bool CanManageInventory { get; set; }
    }

}
