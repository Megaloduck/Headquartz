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
        private string _currentPage = "";

        public string RoleName => _roleService.CurrentRole?.Name ?? "CEO";

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

            // Initialize commands with page names for tracking
            NavigateToDashboardCommand = new RelayCommand(() =>
                LoadPage("Dashboard", () => _services.GetRequiredService<DashboardPage>()));

            NavigateToOverviewCommand = new RelayCommand(() =>
                LoadPage("Overview", () => _services.GetRequiredService<OverviewPage>()));

            NavigateToWarehouseCommand = new RelayCommand(() =>
                LoadPage("Warehouse", () => _services.GetRequiredService<WarehousePage>()));

            NavigateToMarketCommand = new RelayCommand(() =>
                LoadPage("Market", () => _services.GetRequiredService<MarketPage>()));

            NavigateToFinanceCommand = new RelayCommand(() =>
                LoadPage("Finance", () => _services.GetRequiredService<FinancePage>()));

            NavigateToHRCommand = new RelayCommand(() =>
                LoadPage("HR", () => _services.GetRequiredService<HumanResourcePage>()));

            NavigateToSalesCommand = new RelayCommand(() => ShowComingSoon("Sales"));
            NavigateToProductionCommand = new RelayCommand(() => ShowComingSoon("Production"));
            NavigateToLogisticsCommand = new RelayCommand(() => ShowComingSoon("Logistics"));
            NavigateToUsersCommand = new RelayCommand(() => ShowComingSoon("Users"));
            NavigateToSettingsCommand = new RelayCommand(() => ShowComingSoon("Settings"));

            BindingContext = this;

            // Load dashboard by default
            LoadPage("Dashboard", () => _services.GetRequiredService<DashboardPage>());
        }

        private void LoadPage(string pageName, Func<ContentPage> pageFactory)
        {
            try
            {
                // Always clear the current content first
                ContentArea.Content = null;

                // Force a small delay to ensure cleanup
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        // Create a fresh page instance
                        var page = pageFactory();

                        // Get the content
                        var pageContent = page.Content;

                        if (pageContent != null)
                        {
                            // Remove from page to avoid parent conflicts
                            page.Content = null;

                            // Set the new content
                            ContentArea.Content = pageContent;

                            _currentPage = pageName;

                            System.Diagnostics.Debug.WriteLine($"Loaded page: {pageName}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Page {pageName} has no content");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in MainThread: {ex.Message}");
                        DisplayAlert("Error", $"Failed to load {pageName}: {ex.Message}", "OK");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading page {pageName}: {ex.Message}");
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