 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Services
{
    public class RoleService
    {
        public RolePermissions CurrentRole { get; private set; }
        
        public void SetRole(RolePermissions role)
        {
            CurrentRole = role;
        }
    }
}
