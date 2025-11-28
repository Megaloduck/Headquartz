using System;
using System.Collections.Generic;
using Headquartz.Models;

namespace Headquartz.Services
{
    /// <summary>
    /// Available roles in the system.
    /// </summary>
    public enum Role
    {
        CEO,
        HRManager,
        FinanceManager,
        SalesManager,
        WarehouseManager,
        MarketingManager,
        ProductionManager,
        LogisticsManager
    }

    /// <summary>
    /// Permission container for each role.
    /// Every permission is derived automatically from the Role type.
    /// </summary>
    public class RolePermissions
    {
        public Role Role { get; set; }
        public string DisplayName { get; set; }

        // Derived permissions
        public bool IsCEO => Role == Role.CEO;
        public bool IsHRManager => Role == Role.HRManager;
        public bool IsFinanceManager => Role == Role.FinanceManager;
        public bool IsSalesManager => Role == Role.SalesManager;
        public bool IsWarehouseManager => Role == Role.WarehouseManager;
        public bool IsMarketingManager => Role == Role.MarketingManager;
        public bool IsProductionManager => Role == Role.ProductionManager;
        public bool IsLogisticsManager => Role == Role.LogisticsManager;

        public bool IsManager =>
            Role != Role.CEO;
    }

    /// <summary>
    /// Main Role Service used by the entire app.
    /// Provides the logic used by Sidebar & Shell routing.
    /// </summary>
    public class RoleService
    {
        public List<RolePermissions> AvailableRoles { get; private set; }
        public RolePermissions CurrentRole { get; private set; }

        public RoleService()
        {
            AvailableRoles = new List<RolePermissions>
            {
                new RolePermissions { Role = Role.CEO, DisplayName = "CEO" },
                new RolePermissions { Role = Role.HRManager, DisplayName = "HR Manager" },
                new RolePermissions { Role = Role.FinanceManager, DisplayName = "Finance Manager" },
                new RolePermissions { Role = Role.SalesManager, DisplayName = "Sales Manager" },
                new RolePermissions { Role = Role.WarehouseManager, DisplayName = "Warehouse Manager" },
                new RolePermissions { Role = Role.MarketingManager, DisplayName = "Marketing Manager" },
                new RolePermissions { Role = Role.ProductionManager, DisplayName = "Production Manager" },
                new RolePermissions { Role = Role.LogisticsManager, DisplayName = "Logistics Manager" },
            };

            // Default role is CEO
            CurrentRole = AvailableRoles[0];
        }

        /// <summary>
        /// Switches the current active role.
        /// </summary>
        public void SetRole(RolePermissions role)
        {
            if (role == null) return;
            CurrentRole = role;
        }

        // ──────────────────────────────────────────────────────────────
        //  ACCESS CONTROL METHODS (Used by Sidebar & AppShell)
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns true if user is CEO.
        /// </summary>
        public bool IsCEO => CurrentRole.Role == Role.CEO;

        /// <summary>
        /// Returns true for all managers (non-CEO).
        /// </summary>
        public bool IsManager => CurrentRole.Role != Role.CEO;

        /// <summary>
        /// Returns true if user belongs to a specific department.
        /// </summary>
        public bool IsDepartment(string dept)
        {
            return CurrentRole.Role switch
            {
                Role.HRManager => dept == "HR",
                Role.FinanceManager => dept == "Finance",
                Role.SalesManager => dept == "Sales",
                Role.WarehouseManager => dept == "Warehouse",
                Role.MarketingManager => dept == "Marketing",
                Role.ProductionManager => dept == "Production",
                Role.LogisticsManager => dept == "Logistics",
                _ => false
            };
        }

        /// <summary>
        /// CEO: Can access dashboards of all departments
        /// Managers: Can access their own department dashboard only
        /// </summary>
        public bool CanAccessDashboard(string dept)
        {
            if (IsCEO) return true;
            return IsDepartment(dept);
        }

        /// <summary>
        /// CEO: Cannot access tools of any department
        /// Managers: Can access tools of their own department only
        /// </summary>
        public bool CanAccessTools(string dept)
        {
            if (IsCEO) return false;
            return IsDepartment(dept);
        }
    }
}
