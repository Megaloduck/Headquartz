using CommunityToolkit.Mvvm.Input;
using Headquartz.Services;
using Headquartz.PageModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Headquartz.Pages
{
    public partial class SidebarPage : ContentPage, INotifyPropertyChanged
    {
        private readonly RoleService _roleService;
        private readonly ThemeService _themeService;
        private readonly IServiceProvider _services;

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
        public RelayCommand NavigateToOverviewCommand { get; }
        public RelayCommand NavigateToWarehouseCommand { get; }
        public RelayCommand NavigateToSalesCommand { get; }
        public RelayCommand NavigateToMarketCommand { get; }
        public RelayCommand NavigateToProductionCommand { get; }
        public RelayCommand NavigateToLogisticsCommand { get; }
        public RelayCommand NavigateToFinanceCommand { get; }
        public RelayCommand NavigateToHRCommand { get; }
        public RelayCommand NavigateToUsersCommand { get; }
        public RelayCommand NavigateToSettingsCommand { get; }

        public SidebarPage(RoleService roleService, IServiceProvider services)
        {
            InitializeComponent();

            _roleService = roleService;
            _themeService = ThemeService.Instance;
            _services = services;

            _themeService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ThemeService.IsDarkMode))
                {
                    OnPropertyChanged(nameof(IsDarkMode));
                }
            };

            // Initialize commands
            NavigateToDashboardCommand = new RelayCommand(() => LoadPage(() =>
                _services.GetRequiredService<DashboardPage>()));

            NavigateToOverviewCommand = new RelayCommand(() => LoadPage(() =>
                _services.GetRequiredService<OverviewPage>()));

            NavigateToWarehouseCommand = new RelayCommand(() => LoadPage(() =>
                _services.GetRequiredService<WarehousePage>()));

            NavigateToMarketCommand = new RelayCommand(() => LoadPage(() =>
                _services.GetRequiredService<MarketPage>()));

            NavigateToFinanceCommand = new RelayCommand(() => LoadPage(() =>
                _services.GetRequiredService<FinancePage>()));

            NavigateToHRCommand = new RelayCommand(() => LoadPage(() =>
                _services.GetRequiredService<HumanResourcePage>()));

            NavigateToSalesCommand = new RelayCommand(() => ShowComingSoon("Sales"));
            NavigateToProductionCommand = new RelayCommand(() => ShowComingSoon("Production"));
            NavigateToLogisticsCommand = new RelayCommand(() => ShowComingSoon("Logistics"));
            NavigateToUsersCommand = new RelayCommand(() => ShowComingSoon("Users"));
            NavigateToSettingsCommand = new RelayCommand(() => ShowComingSoon("Settings"));

            BindingContext = this;

            // Load dashboard by default
            LoadPage(() => _services.GetRequiredService<DashboardPage>());
        }

        private void LoadPage(Func<ContentPage> pageFactory)
        {
            try
            {
                // Clear current content
                ContentArea.Content = null;

                // Create the page
                var page = pageFactory();

                // Extract the page content
                var pageContent = page.Content;
                page.Content = null; // Remove from page to avoid conflicts

                // Load into content area
                ContentArea.Content = pageContent;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading page: {ex.Message}");
                DisplayAlert("Error", $"Failed to load page: {ex.Message}", "OK");
            }
        }

        private async void ShowComingSoon(string moduleName)
        {
            await DisplayAlert("Coming Soon",
                $"The {moduleName} module is under development and will be available soon.",
                "OK");
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}