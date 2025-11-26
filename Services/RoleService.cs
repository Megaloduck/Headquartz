using System;
using System.Collections.Generic;
using Headquartz.Models;

namespace Headquartz.Services
{
    public class RoleService
    {
        // List of available roles for the dropdown
        public List<RolePermissions> AvailableRoles { get; private set; }

        // Currently active role
        public RolePermissions CurrentRole { get; private set; }

        public RoleService()
        {
            // Initialize available roles
            AvailableRoles = new List<RolePermissions>
            {
                new RolePermissions { Name = "CEO" },
                new RolePermissions { Name = "HR Manager" },
                new RolePermissions { Name = "Finance Manager" },
                new RolePermissions { Name = "Sales Manager" },
                new RolePermissions { Name = "Warehouse Manager" },
                new RolePermissions { Name = "Marketing Manager" },
                new RolePermissions { Name = "Production Manager" },
                new RolePermissions { Name = "Logistics Manager" }
            };

            // Default role
            CurrentRole = AvailableRoles[0];
        }

        // Switch role
        public void SetRole(RolePermissions role)
        {
            if (role == null) return;
            CurrentRole = role;
        }
    }
}
