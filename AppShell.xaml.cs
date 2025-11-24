using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;
using Headquartz.Models;
using Headquartz.Pages;
using Headquartz.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Headquartz
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        private readonly RoleService _roleService;
        private readonly ThemeService _themeService;

        public string RoleName => _roleService.CurrentRole?.Name ?? "Guest";

        public bool IsDarkMode
        {
            get => _themeService.IsDarkMode;
            set
            {
                if (_themeService.IsDarkMode != value)
                {
                    _themeService.IsDarkMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public AppShell(RoleService roleService)
        {
            InitializeComponent();

            _roleService = roleService;
            _themeService = ThemeService.Instance;

            // Subscribe to theme changes
            _themeService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ThemeService.IsDarkMode))
                {
                    OnPropertyChanged(nameof(IsDarkMode));
                }
            };

            // Set default role (CEO has access to everything)
            _roleService.SetRole(new RolePermissions
            {
                Name = "CEO",
                CanSeeFinance = true,
                CanManageHR = true,
                CanSeeMarket = true,
                CanManageInventory = true
            });

            BuildMenu();

            BindingContext = this;
        }

        private void BuildMenu()
        {
            var role = _roleService.CurrentRole;

            // Clear existing items
            Items.Clear();

            // Dashboard (everyone has access)
            Items.Add(new FlyoutItem
            {
                Title = "Dashboard",
                Route = "dashboard",
                Icon = "📊",
                Items =
                {
                    new ShellContent
                    {
                        ContentTemplate = new DataTemplate(typeof(DashboardPage)),
                        Route = "dashboardpage"
                    }
                }
            });

            // Main Page
            Items.Add(new FlyoutItem
            {
                Title = "Overview",
                Route = "main",
                Icon = "🏠",
                Items =
                {
                    new ShellContent
                    {
                        ContentTemplate = new DataTemplate(typeof(MainPage)),
                        Route = "mainpage"
                    }
                }
            });

            // Inventory (if role permits)
            if (role.CanManageInventory)
            {
                Items.Add(new FlyoutItem
                {
                    Title = "Inventory",
                    Route = "inventory",
                    Icon = "📦",
                    Items =
                    {
                        new ShellContent
                        {
                            ContentTemplate = new DataTemplate(typeof(InventoryPage)),
                            Route = "inventorypage"
                        }
                    }
                });
            }

            // Market (if role permits)
            if (role.CanSeeMarket)
            {
                Items.Add(new FlyoutItem
                {
                    Title = "Market",
                    Route = "market",
                    Icon = "📊",
                    Items =
                    {
                        new ShellContent
                        {
                            ContentTemplate = new DataTemplate(typeof(MarketPage)),
                            Route = "marketpage"
                        }
                    }
                });
            }

            // Finance (if role permits)
            if (role.CanSeeFinance)
            {
                Items.Add(new FlyoutItem
                {
                    Title = "Finance",
                    Route = "finance",
                    Icon = "💰",
                    Items =
                    {
                        new ShellContent
                        {
                            ContentTemplate = new DataTemplate(typeof(FinancePage)),
                            Route = "financepage"
                        }
                    }
                });
            }

            // Human Resources (if role permits)
            if (role.CanManageHR)
            {
                Items.Add(new FlyoutItem
                {
                    Title = "Human Resources",
                    Route = "hr",
                    Icon = "👥",
                    Items =
                    {
                        new ShellContent
                        {
                            ContentTemplate = new DataTemplate(typeof(HumanResourcePage)),
                            Route = "hrpage"
                        }
                    }
                });
            }

            OnPropertyChanged(nameof(RoleName));
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Switch to a different role (useful for testing or role changes)
        /// </summary>
        public void SwitchRole(RolePermissions newRole)
        {
            _roleService.SetRole(newRole);
            BuildMenu();
        }
    }
}