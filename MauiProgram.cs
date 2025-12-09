using CommunityToolkit.Maui;
using Headquartz.Models;
using Headquartz.PageModels;
using Headquartz.PageModels.Warehouse;
using Headquartz.Pages;
using Headquartz.Pages.CEO;
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
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace Headquartz
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts => { });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ────────────────────────────────────────────────
            // CORE GAME SERVICES
            // ────────────────────────────────────────────────
            builder.Services.AddSingleton<GameState>();
            //builder.Services.AddSingleton<ISimulationEngine, SimulationEngine>();
            builder.Services.AddSingleton<ISaveService, JsonSaveService>();

            // Role Service (global access everywhere)
            builder.Services.AddSingleton<RoleService>();

            // Sidebar - must stay alive during the entire app lifetime
            builder.Services.AddSingleton<SidebarCEOPage>();
            builder.Services.AddSingleton<SidebarHRPage>();
            builder.Services.AddSingleton<SidebarWarehousePage>();
            builder.Services.AddSingleton<SidebarFinancePage>();
            builder.Services.AddSingleton<SidebarProductionPage>();
            builder.Services.AddSingleton<SidebarSalesPage>();
            builder.Services.AddSingleton<SidebarMarketingPage>();
            builder.Services.AddSingleton<SidebarLogisticsPage>();

            // Core Fundamental Pages
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginPageModel>();
            builder.Services.AddSingleton<BasePlatePage>();

            // ────────────────────────────────────────────────
            // COMPANY-WIDE PAGES
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<CompanyDashboardPage>();
            builder.Services.AddTransient<OverviewPage>();

            // ────────────────────────────────────────────────
            // WAREHOUSE MODULE
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<WarehouseDashboardPage>();
            builder.Services.AddTransient<WarehouseDashboardPageModel>();
            builder.Services.AddTransient<InventoryPage>();
            builder.Services.AddTransient<InventoryPageModel>();
            builder.Services.AddTransient<StockInPage>();
            builder.Services.AddTransient<StockInPageModel>();
            builder.Services.AddTransient<StockOutPage>();
            builder.Services.AddTransient<StockOutPageModel>();
            builder.Services.AddTransient<ShipmentsPage>();
            builder.Services.AddTransient<ShipmentsPageModel>();
            builder.Services.AddTransient<StorageAllocationPage>();
            builder.Services.AddTransient<StorageAllocationPageModel>();
            builder.Services.AddTransient<WarehouseReportsPage>();
            builder.Services.AddTransient<WarehouseReportsPageModel>();

            // ────────────────────────────────────────────────
            // PRODUCTION MODULE
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<ProductionDashboardPage>();
            builder.Services.AddTransient<WorkOrdersPage>();
            builder.Services.AddTransient<FactorySchedulePage>();
            builder.Services.AddTransient<RawMaterialsPage>();
            builder.Services.AddTransient<MachinesPage>();
            builder.Services.AddTransient<ProductionQualityPage>();
            builder.Services.AddTransient<ProductionReportsPage>();

            // ────────────────────────────────────────────────
            // SALES MODULE
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<SalesDashboardPage>();
            builder.Services.AddTransient<LeadsPage>();
            builder.Services.AddTransient<PricingPage>();
            builder.Services.AddTransient<OrdersPage>();
            builder.Services.AddTransient<SalesTargetsPage>();
            builder.Services.AddTransient<CustomersPage>();
            builder.Services.AddTransient<SalesReportsPage>();

            // ────────────────────────────────────────────────
            // MARKETING MODULE
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<MarketingDashboardPage>();
            builder.Services.AddTransient<CampaignsPage>();
            builder.Services.AddTransient<MarketResearchPage>();
            builder.Services.AddTransient<PricingStrategyPage>();
            builder.Services.AddTransient<CompetitorAnalysisPage>();
            builder.Services.AddTransient<BrandingPage>();
            builder.Services.AddTransient<MarketingReportsPage>();

            // ────────────────────────────────────────────────
            // FINANCE MODULE
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<FinanceDashboardPage>();
            builder.Services.AddTransient<BudgetPage>();
            builder.Services.AddTransient<ExpensesPage>();
            builder.Services.AddTransient<RevenuePage>();
            builder.Services.AddTransient<CashFlowPage>();
            builder.Services.AddTransient<FinancialStatementsPage>();
            builder.Services.AddTransient<FinanceReportsPage>();

            // ────────────────────────────────────────────────
            // HUMAN RESOURCE MODULE
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<HRDashboardPage>();
            builder.Services.AddTransient<EmployeeListPage>();
            builder.Services.AddTransient<RecruitmentPage>();
            builder.Services.AddTransient<PayrollPage>();
            builder.Services.AddTransient<TrainingPage>();
            builder.Services.AddTransient<HRPoliciesPage>();
            builder.Services.AddTransient<HRReportsPage>();

            // ────────────────────────────────────────────────
            // LOGISTICS MODULE
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<LogisticsDashboardPage>();
            builder.Services.AddTransient<ShippingPage>();
            builder.Services.AddTransient<DeliveryTrackingPage>();
            builder.Services.AddTransient<RoutingPage>();
            builder.Services.AddTransient<SupplierManagementPage>();
            builder.Services.AddTransient<FleetManagementPage>();
            builder.Services.AddTransient<LogisticsReportsPage>();

            // ────────────────────────────────────────────────
            // SYSTEM MODULE
            // ────────────────────────────────────────────────
            builder.Services.AddTransient<UsersPage>();
            builder.Services.AddTransient<SettingsPage>();

            // ────────────────────────────────────────────────
            // BUILD APP
            // ────────────────────────────────────────────────
            var app = builder.Build();

            // Seed initial game data
            var gameState = app.Services.GetRequiredService<GameState>();
            //SeedService.Seed(gameState);

            return app;
        }
    }
}
