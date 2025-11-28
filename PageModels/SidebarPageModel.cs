using Headquartz.Modules.HumanResource;
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
            Roles = _roleService.AvailableRoles;

            BuildCommands();
        }

        public RolePermissions SelectedRole
        {
            get => _roleService.CurrentRole;
            set
            {
                if (value != null)
                {
                    _roleService.SetRole(value);
                    OnPropertyChanged();
                    UpdateSidebarVisibility();
                }
            }
        }

        public List<RolePermissions> Roles { get; }

        // Visibility flags
        public bool IsCEO => _roleService.IsCEO;
        public bool IsManager => _roleService.IsManager;
        public bool IsWarehouseManager => SelectedRole.IsWarehouseManager;
        public bool IsHRManager => SelectedRole.IsHRManager;
        public bool IsFinanceManager => SelectedRole.IsFinanceManager;
        public bool IsSalesManager => SelectedRole.IsSalesManager;
        public bool IsMarketingManager => SelectedRole.IsMarketingManager;
        public bool IsProductionManager => SelectedRole.IsProductionManager;
        public bool IsLogisticsManager => SelectedRole.IsLogisticsManager;

        private void UpdateSidebarVisibility()
        {
            OnPropertyChanged(nameof(IsCEO));
            OnPropertyChanged(nameof(IsManager));
            OnPropertyChanged(nameof(IsWarehouseManager));
            OnPropertyChanged(nameof(IsHRManager));
            OnPropertyChanged(nameof(IsFinanceManager));
            OnPropertyChanged(nameof(IsSalesManager));
            OnPropertyChanged(nameof(IsMarketingManager));
            OnPropertyChanged(nameof(IsProductionManager));
            OnPropertyChanged(nameof(IsLogisticsManager));
        }

        // Main Menu Commands
        public ICommand NavigateToDashboardCommand { get; private set; }
        public ICommand NavigateToOverviewCommand { get; private set; }

        // CEO Department Dashboard Commands
        public ICommand NavigateToWarehouseCommand { get; private set; }
        public ICommand NavigateToSalesCommand { get; private set; }
        public ICommand NavigateToMarketCommand { get; private set; }
        public ICommand NavigateToProductionCommand { get; private set; }
        public ICommand NavigateToLogisticsCommand { get; private set; }
        public ICommand NavigateToFinanceCommand { get; private set; }
        public ICommand NavigateToHRCommand { get; private set; }

        // Warehouse Manager Tools
        public ICommand NavigateToWarehouseDashboardCommand { get; private set; }
        public ICommand NavigateToInventoryCommand { get; private set; }
        public ICommand NavigateToStockInCommand { get; private set; }
        public ICommand NavigateToStockOutCommand { get; private set; }
        public ICommand NavigateToWarehouseReportsCommand { get; private set; }

        // HR Manager Tools
        public ICommand NavigateToHRDashboardCommand { get; private set; }
        public ICommand NavigateToEmployeeListCommand { get; private set; }
        public ICommand NavigateToRecruitmentCommand { get; private set; }
        public ICommand NavigateToPayrollCommand { get; private set; }
        public ICommand NavigateToTrainingCommand { get; private set; }
        public ICommand NavigateToHRReportsCommand { get; private set; }

        // Finance Manager Tools
        public ICommand NavigateToFinanceDashboardCommand { get; private set; }
        public ICommand NavigateToBudgetCommand { get; private set; }
        public ICommand NavigateToExpensesCommand { get; private set; }
        public ICommand NavigateToRevenueCommand { get; private set; }
        public ICommand NavigateToFinanceReportsCommand { get; private set; }

        // Sales Manager Tools
        public ICommand NavigateToSalesDashboardCommand { get; private set; }
        public ICommand NavigateToLeadsCommand { get; private set; }
        public ICommand NavigateToOrdersCommand { get; private set; }
        public ICommand NavigateToCustomersCommand { get; private set; }
        public ICommand NavigateToSalesReportsCommand { get; private set; }

        // Marketing Manager Tools
        public ICommand NavigateToMarketingDashboardCommand { get; private set; }
        public ICommand NavigateToCampaignsCommand { get; private set; }
        public ICommand NavigateToMarketResearchCommand { get; private set; }
        public ICommand NavigateToMarketingReportsCommand { get; private set; }

        // Production Manager Tools
        public ICommand NavigateToProductionDashboardCommand { get; private set; }
        public ICommand NavigateToWorkOrdersCommand { get; private set; }
        public ICommand NavigateToMachinesCommand { get; private set; }
        public ICommand NavigateToProductionReportsCommand { get; private set; }

        // Logistics Manager Tools
        public ICommand NavigateToLogisticsDashboardCommand { get; private set; }
        public ICommand NavigateToShippingCommand { get; private set; }
        public ICommand NavigateToDeliveryTrackingCommand { get; private set; }
        public ICommand NavigateToLogisticsReportsCommand { get; private set; }

        // System Commands
        public ICommand NavigateToUsersCommand { get; private set; }
        public ICommand NavigateToSettingsCommand { get; private set; }

        private void BuildCommands()
        {
            // Main Menu
            NavigateToDashboardCommand = new Command(() => NavigateAction?.Invoke(new CompanyDashboardPage()));
            NavigateToOverviewCommand = new Command(() => NavigateAction?.Invoke(new OverviewPage()));

            // CEO Department Dashboards
            NavigateToWarehouseCommand = new Command(() => NavigateAction?.Invoke(new WarehouseDashboardPage()));
            NavigateToSalesCommand = new Command(() => NavigateAction?.Invoke(new SalesDashboardPage()));
            NavigateToMarketCommand = new Command(() => NavigateAction?.Invoke(new MarketingDashboardPage()));
            NavigateToProductionCommand = new Command(() => NavigateAction?.Invoke(new ProductionDashboardPage()));
            NavigateToLogisticsCommand = new Command(() => NavigateAction?.Invoke(new LogisticsDashboardPage()));
            NavigateToFinanceCommand = new Command(() => NavigateAction?.Invoke(new FinanceDashboardPage()));
            NavigateToHRCommand = new Command(() => NavigateAction?.Invoke(new HRDashboardPage()));

            // Warehouse Manager Tools
            NavigateToWarehouseDashboardCommand = new Command(() => NavigateAction?.Invoke(new WarehouseDashboardPage()));
            NavigateToInventoryCommand = new Command(() => NavigateAction?.Invoke(new InventoryPage()));
            NavigateToStockInCommand = new Command(() => NavigateAction?.Invoke(new StockInPage()));
            NavigateToStockOutCommand = new Command(() => NavigateAction?.Invoke(new StockOutPage()));
            NavigateToWarehouseReportsCommand = new Command(() => NavigateAction?.Invoke(new WarehouseReportsPage()));

            // HR Manager Tools
            NavigateToHRDashboardCommand = new Command(() => NavigateAction?.Invoke(new HRDashboardPage()));
            NavigateToEmployeeListCommand = new Command(() => NavigateAction?.Invoke(new EmployeeListPage()));
            NavigateToRecruitmentCommand = new Command(() => NavigateAction?.Invoke(new RecruitmentPage()));
            NavigateToPayrollCommand = new Command(() => NavigateAction?.Invoke(new PayrollPage()));
            NavigateToTrainingCommand = new Command(() => NavigateAction?.Invoke(new TrainingPage()));
            NavigateToHRReportsCommand = new Command(() => NavigateAction?.Invoke(new HRReportsPage()));

            // Finance Manager Tools
            NavigateToFinanceDashboardCommand = new Command(() => NavigateAction?.Invoke(new FinanceDashboardPage()));
            NavigateToBudgetCommand = new Command(() => NavigateAction?.Invoke(new BudgetPage()));
            NavigateToExpensesCommand = new Command(() => NavigateAction?.Invoke(new ExpensesPage()));
            NavigateToRevenueCommand = new Command(() => NavigateAction?.Invoke(new RevenuePage()));
            NavigateToFinanceReportsCommand = new Command(() => NavigateAction?.Invoke(new FinanceReportsPage()));

            // Sales Manager Tools
            NavigateToSalesDashboardCommand = new Command(() => NavigateAction?.Invoke(new SalesDashboardPage()));
            NavigateToLeadsCommand = new Command(() => NavigateAction?.Invoke(new LeadsPage()));
            NavigateToOrdersCommand = new Command(() => NavigateAction?.Invoke(new OrdersPage()));
            NavigateToCustomersCommand = new Command(() => NavigateAction?.Invoke(new CustomersPage()));
            NavigateToSalesReportsCommand = new Command(() => NavigateAction?.Invoke(new SalesReportsPage()));

            // Marketing Manager Tools
            NavigateToMarketingDashboardCommand = new Command(() => NavigateAction?.Invoke(new MarketingDashboardPage()));
            NavigateToCampaignsCommand = new Command(() => NavigateAction?.Invoke(new CampaignsPage()));
            NavigateToMarketResearchCommand = new Command(() => NavigateAction?.Invoke(new MarketResearchPage()));
            NavigateToMarketingReportsCommand = new Command(() => NavigateAction?.Invoke(new MarketingReportsPage()));

            // Production Manager Tools
            NavigateToProductionDashboardCommand = new Command(() => NavigateAction?.Invoke(new ProductionDashboardPage()));
            NavigateToWorkOrdersCommand = new Command(() => NavigateAction?.Invoke(new WorkOrdersPage()));
            NavigateToMachinesCommand = new Command(() => NavigateAction?.Invoke(new MachinesPage()));
            NavigateToProductionReportsCommand = new Command(() => NavigateAction?.Invoke(new ProductionReportsPage()));

            // Logistics Manager Tools
            NavigateToLogisticsDashboardCommand = new Command(() => NavigateAction?.Invoke(new LogisticsDashboardPage()));
            NavigateToShippingCommand = new Command(() => NavigateAction?.Invoke(new ShippingPage()));
            NavigateToDeliveryTrackingCommand = new Command(() => NavigateAction?.Invoke(new DeliveryTrackingPage()));
            NavigateToLogisticsReportsCommand = new Command(() => NavigateAction?.Invoke(new LogisticsReportsPage()));

            // System
            NavigateToUsersCommand = new Command(() => NavigateAction?.Invoke(new UsersPage()));
            NavigateToSettingsCommand = new Command(() => NavigateAction?.Invoke(new SettingsPage()));
        }
    }
}