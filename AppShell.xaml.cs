using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using Headquartz.Pages;
using Headquartz.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Font = Microsoft.Maui.Font;

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

        // Navigation Commands
        public RelayCommand NavigateToDashboardCommand { get; }
        public RelayCommand NavigateToMainCommand { get; }
        public RelayCommand NavigateToInventoryCommand { get; }
        public RelayCommand NavigateToMarketCommand { get; }
        public RelayCommand NavigateToFinanceCommand { get; }
        public RelayCommand NavigateToHRCommand { get; }

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

            // Initialize navigation commands
            NavigateToDashboardCommand = new RelayCommand(async () => await NavigateToAsync("//dashboard"));
            NavigateToMainCommand = new RelayCommand(async () => await NavigateToAsync("//main"));
            NavigateToInventoryCommand = new RelayCommand(async () => await NavigateToAsync("//inventory"));
            NavigateToMarketCommand = new RelayCommand(async () => await NavigateToAsync("//market"));
            NavigateToFinanceCommand = new RelayCommand(async () => await NavigateToAsync("//finance"));
            NavigateToHRCommand = new RelayCommand(async () => await NavigateToAsync("//hr"));

            RegisterRoutes();
            UpdateSidebarVisibility();

            BindingContext = this;
        }

        private void RegisterRoutes()
        {
            // Register all routes
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("main", typeof(MainPage));
            Routing.RegisterRoute("warehouse", typeof(WarehousePage));
            Routing.RegisterRoute("market", typeof(MarketPage));
            Routing.RegisterRoute("finance", typeof(FinancePage));
            Routing.RegisterRoute("hr", typeof(HumanResourcePage));
        }

        private async Task NavigateToAsync(string route)
        {
            try
            {
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }

        private void UpdateSidebarVisibility()
        {
            var role = _roleService.CurrentRole;

            // Update visibility based on role permissions
            if (InventoryItem != null)
                InventoryItem.IsVisible = role.CanManageInventory;

            if (MarketItem != null)
                MarketItem.IsVisible = role.CanSeeMarket;

            if (FinanceItem != null)
                FinanceItem.IsVisible = role.CanSeeFinance;

            if (HRItem != null)
                HRItem.IsVisible = role.CanManageHR;

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
            UpdateSidebarVisibility();
        }
    }
}