using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
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

        // Navigation Commands - Main
        public RelayCommand NavigateToDashboardCommand { get; }
        public RelayCommand NavigateToMainCommand { get; }

        // Navigation Commands - Modules
        public RelayCommand NavigateToWarehouseCommand { get; }
        public RelayCommand NavigateToSalesCommand { get; }
        public RelayCommand NavigateToMarketCommand { get; }
        public RelayCommand NavigateToProductionCommand { get; }
        public RelayCommand NavigateToLogisticsCommand { get; }
        public RelayCommand NavigateToFinanceCommand { get; }
        public RelayCommand NavigateToHRCommand { get; }

        // Navigation Commands - System
        public RelayCommand NavigateToUsersCommand { get; }
        public RelayCommand NavigateToSettingsCommand { get; }

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
                CanManageWarehouse = true
            });

            // Initialize navigation commands - Main
            NavigateToDashboardCommand = new RelayCommand(async () => await NavigateToAsync("//dashboard"));
            NavigateToMainCommand = new RelayCommand(async () => await NavigateToAsync("//main"));

            // Initialize navigation commands - Modules
            NavigateToWarehouseCommand = new RelayCommand(async () => await NavigateToAsync("//warehouse"));
            NavigateToSalesCommand = new RelayCommand(async () => await ShowComingSoonAsync("Sales"));
            NavigateToMarketCommand = new RelayCommand(async () => await NavigateToAsync("//market"));
            NavigateToProductionCommand = new RelayCommand(async () => await ShowComingSoonAsync("Production"));
            NavigateToLogisticsCommand = new RelayCommand(async () => await ShowComingSoonAsync("Logistics"));
            NavigateToFinanceCommand = new RelayCommand(async () => await NavigateToAsync("//finance"));
            NavigateToHRCommand = new RelayCommand(async () => await NavigateToAsync("//hr"));

            // Initialize navigation commands - System
            NavigateToUsersCommand = new RelayCommand(async () => await ShowComingSoonAsync("Users"));
            NavigateToSettingsCommand = new RelayCommand(async () => await ShowComingSoonAsync("Settings"));

            UpdateSidebarVisibility();

            BindingContext = this;
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

        private async Task ShowComingSoonAsync(string moduleName)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Coming Soon",
                $"The {moduleName} module is under development and will be available soon.",
                "OK");
        }

        private void UpdateSidebarVisibility()
        {
            var role = _roleService.CurrentRole;

            // Update visibility in sidebar
            if (WarehouseItem != null)
                WarehouseItem.IsVisible = role.CanManageInventory;

            if (MarketItem != null)
                MarketItem.IsVisible = role.CanSeeMarket;

            if (FinanceItem != null)
                FinanceItem.IsVisible = role.CanSeeFinance;

            if (HRItem != null)
                HRItem.IsVisible = role.CanManageHR;

            // Sales, Production, Logistics are always visible (or add permissions)
            // Users and Settings are always visible for admins

            // Update visibility in FlyoutItems (for navigation to work)
            if (WarehouseFlyoutItem != null)
                WarehouseFlyoutItem.IsVisible = role.CanManageWarehouse;

            if (MarketFlyoutItem != null)
                MarketFlyoutItem.IsVisible = role.CanSeeMarket;

            if (FinanceFlyoutItem != null)
                FinanceFlyoutItem.IsVisible = role.CanSeeFinance;

            if (HRFlyoutItem != null)
                HRFlyoutItem.IsVisible = role.CanManageHR;

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