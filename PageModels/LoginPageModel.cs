using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Pages.CEO;
using Headquartz.Pages.Finance;
using Headquartz.Pages.HumanResource;
using Headquartz.Pages.Logistics;
using Headquartz.Pages.Marketing;
using Headquartz.Pages.Production;
using Headquartz.Pages.Sales;
using Headquartz.Pages.Warehouse;
using Headquartz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.PageModels
{
    public partial class LoginPageModel : ObservableObject
    {
        private readonly RoleService _roleService;

        public List<RolePermissions> Roles { get; }

        [ObservableProperty]
        private RolePermissions selectedRole;

        public IRelayCommand LoginCommand { get; }

        public LoginPageModel(RoleService roleService)
        {
            _roleService = roleService;

            Roles = roleService.AvailableRoles;

            SelectedRole = Roles[0]; // default

            LoginCommand = new RelayCommand(OnLogin);
        }

        private async void OnLogin()
        {
            if (SelectedRole == null)
                return;

            _roleService.SetRole(SelectedRole);

            string targetPage = SelectedRole.Role switch
            {
                Role.CEO => nameof(SidebarCEOPage),
                Role.HRManager => nameof(SidebarHRPage),
                Role.FinanceManager => nameof(SidebarFinancePage),
                Role.SalesManager => nameof(SidebarSalesPage),
                Role.MarketingManager => nameof(SidebarMarketingPage),
                Role.ProductionManager => nameof(SidebarProductionPage),
                Role.WarehouseManager => nameof(SidebarWarehousePage),
                Role.LogisticsManager => nameof(SidebarLogisticsPage),
                _ => nameof(SidebarCEOPage)
            };

            await Shell.Current.GoToAsync($"//{targetPage}");
        }
    }
}
