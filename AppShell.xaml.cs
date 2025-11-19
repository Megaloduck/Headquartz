using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;
using Headquartz.Models;
using Headquartz.Pages;
using Headquartz.Services;


namespace Headquartz
{
    public partial class AppShell : Shell
    {
        private readonly RoleService _roleService;

        public AppShell()
        {
            InitializeComponent();

            // Get service from DI
            _roleService = Handler?.MauiContext?.Services.GetService<RoleService>()
                          ?? new RoleService();

            // Set a default role
            _roleService.SetRole(new RolePermissions
            {
                Role = PlayerRole.CEO,
                CanSeeFinance = true,
                CanManageHR = true,
                CanSeeMarket = true,
                CanManageInventory = true
            });

            BuildMenu();
        }

        private void BuildMenu()
        {
            var role = _roleService.CurrentRole;

            Items.Clear();

            // Dashboard (everyone)
            Items.Add(new FlyoutItem
            {
                Title = "Dashboard",
                Items = { new ShellContent { ContentTemplate = new DataTemplate(typeof(DashboardPage)) } }
            });

            if (role.CanManageHR)
            {
                Items.Add(new FlyoutItem
                {
                    Title = "Human Resources",
                    Items = { new ShellContent { ContentTemplate = new DataTemplate(typeof(HumanResourcePage)) } }
                });
            }

            if (role.CanSeeFinance)
            {
                Items.Add(new FlyoutItem
                {
                    Title = "Finance",
                    Items = { new ShellContent { ContentTemplate = new DataTemplate(typeof(FinancePage)) } }
                });
            }

            if (role.CanManageInventory)
            {
                Items.Add(new FlyoutItem
                {
                    Title = "Inventory",
                    Items = { new ShellContent { ContentTemplate = new DataTemplate(typeof(InventoryPage)) } }
                });
            }

            if (role.CanSeeMarket)
            {
                Items.Add(new FlyoutItem
                {
                    Title = "Market",
                    Items = { new ShellContent { ContentTemplate = new DataTemplate(typeof(MarketPage)) } }
                });
            }
        }
    }
}
