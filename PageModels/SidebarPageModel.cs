using Headquartz.Pages.Dashboard;
using Headquartz.Pages.Finance;
using Headquartz.Pages.HumanResource;
using Headquartz.Pages.Logistics;
using Headquartz.Pages.Marketing;
using Headquartz.Pages.Production;
using Headquartz.Pages.Sales;
using Headquartz.Pages.System;
using Headquartz.Pages.Warehouse;
using Headquartz.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace Headquartz.PageModels
{
    public class SidebarPageModel : BasePageModel
    {
        private readonly RoleService _roleService;

        public Action<Page> NavigateAction { get; set; }

        public SidebarPageModel(RoleService roleService)
        {
            _roleService = roleService;

            SelectedRole = _roleService.CurrentRole;
            Roles = _roleService.Roles;

            BuildCommands();
        }

        public RoleModel SelectedRole
        {
            get => _roleService.CurrentRole;
            set
            {
                if (value != null)
                {
                    _roleService.SetRole(value.Name);
                    OnPropertyChanged();
                    UpdateSidebarVisibility();
                }
            }
        }

        public List<Role> Roles { get; }

        // Visibility flags
        public bool IsCEO => SelectedRole.Name == "CEO";
        public bool IsManager => !IsCEO;

        private void UpdateSidebarVisibility()
        {
            OnPropertyChanged(nameof(IsCEO));
            OnPropertyChanged(nameof(IsManager));
        }

        // Commands
        public ICommand NavigateToDashboardCommand { get; private set; }
        public ICommand NavigateToOverviewCommand { get; private set; }
        public ICommand NavigateToWarehouseCommand { get; private set; }
        public ICommand NavigateToSalesCommand { get; private set; }
        public ICommand NavigateToMarketCommand { get; private set; }
        public ICommand NavigateToProductionCommand { get; private set; }
        public ICommand NavigateToLogisticsCommand { get; private set; }
        public ICommand NavigateToFinanceCommand { get; private set; }
        public ICommand NavigateToHRCommand { get; private set; }
        public ICommand NavigateToUsersCommand { get; private set; }
        public ICommand NavigateToSettingsCommand { get; private set; }

        private void BuildCommands()
        {
            NavigateToDashboardCommand = new Command(() => NavigateAction?.Invoke(new CompanyDashboardPage()));
            NavigateToOverviewCommand = new Command(() => NavigateAction?.Invoke(new OverviewPage()));

            NavigateToWarehouseCommand = new Command(() => NavigateAction?.Invoke(new WarehouseDashboardPage()));
            NavigateToSalesCommand = new Command(() => NavigateAction?.Invoke(new SalesDashboardPage()));
            NavigateToMarketCommand = new Command(() => NavigateAction?.Invoke(new MarketingDashboardPage()));
            NavigateToProductionCommand = new Command(() => NavigateAction?.Invoke(new ProductionDashboardPage()));
            NavigateToLogisticsCommand = new Command(() => NavigateAction?.Invoke(new LogisticsDashboardPage()));
            NavigateToFinanceCommand = new Command(() => NavigateAction?.Invoke(new FinanceDashboardPage()));
            NavigateToHRCommand = new Command(() => NavigateAction?.Invoke(new HRDashboardPage()));

            NavigateToUsersCommand = new Command(() => NavigateAction?.Invoke(new UsersPage()));
            NavigateToSettingsCommand = new Command(() => NavigateAction?.Invoke(new SettingsPage()));
        }
    }
}
