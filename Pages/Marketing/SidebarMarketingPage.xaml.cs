using CommunityToolkit.Mvvm.Input;
using Headquartz.Pages.Dashboard;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Headquartz.Services;
using Headquartz.Models;
using Headquartz.Pages.System;
namespace Headquartz.Pages.Marketing;

public partial class SidebarMarketingPage : ContentPage
{
    private readonly RoleService _roleService;
    private readonly ThemeService _themeService;
    private readonly IServiceProvider _services;
    private string _currentPage = "";

    // Displayed role name in UI
    //public string RoleName => _roleService.CurrentRole?.DisplayName ?? "Sales Manager";

    // ROLE PICKER BINDING
    public List<RolePermissions> Roles => _roleService.AvailableRoles;

    private RolePermissions _selectedRole;
    public RolePermissions SelectedRole
    {
        get => _selectedRole;
        set
        {
            if (_selectedRole != value)
            {
                _selectedRole = value;

                // Apply new role
                _roleService.SetRole(_selectedRole);

                // Refresh UI
                // OnPropertyChanged(nameof(RoleName));
                OnPropertyChanged();
            }
        }
    }

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
    public RelayCommand NavigateToMarketingDashboardCommand { get; }
    public RelayCommand NavigateToCampaignsCommand { get; }
    public RelayCommand NavigateToMarketResearchCommand { get; }
    public RelayCommand NavigateToPricingStrategyCommand { get; }
    public RelayCommand NavigateToCompetitorAnalysisCommand { get; }
    public RelayCommand NavigateToBrandingCommand { get; }
    public RelayCommand NavigateToMarketingReportsCommand { get; }
    public RelayCommand NavigateToUsersCommand { get; }
    public RelayCommand NavigateToSettingsCommand { get; }

    public SidebarMarketingPage(RoleService roleService, IServiceProvider services)
    {
        InitializeComponent();

        _roleService = roleService;
        _themeService = ThemeService.Instance;
        _services = services;

        // Initialize selected role
        _selectedRole = _roleService.CurrentRole;

        _themeService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ThemeService.IsDarkMode))
            {
                OnPropertyChanged(nameof(IsDarkMode));
            }
        };

        // Initialize commands with page names for tracking
        NavigateToDashboardCommand = new RelayCommand(() =>
            LoadPage("Dashboard", () => _services.GetRequiredService<CompanyDashboardPage>()));

        NavigateToOverviewCommand = new RelayCommand(() =>
            LoadPage("Overview", () => _services.GetRequiredService<OverviewPage>()));

        NavigateToMarketingDashboardCommand = new RelayCommand(() =>
            LoadPage("Marketing Dashboard", () => _services.GetRequiredService<MarketingDashboardPage>()));

        NavigateToCampaignsCommand = new RelayCommand(() =>
            LoadPage("Campaigns", () => _services.GetRequiredService<CampaignsPage>()));

        NavigateToMarketResearchCommand = new RelayCommand(() =>
            LoadPage("Market Research", () => _services.GetRequiredService<MarketResearchPage>()));

        NavigateToPricingStrategyCommand = new RelayCommand(() =>
            LoadPage("Pricing Strategy", () => _services.GetRequiredService<PricingStrategyPage>()));

        NavigateToCompetitorAnalysisCommand = new RelayCommand(() =>
            LoadPage("Competitor Analysis", () => _services.GetRequiredService<CompetitorAnalysisPage>()));

        NavigateToBrandingCommand = new RelayCommand(() =>
            LoadPage("Branding", () => _services.GetRequiredService<BrandingPage>()));

        NavigateToMarketingReportsCommand = new RelayCommand(() =>
            LoadPage("Marketing Reports", () => _services.GetRequiredService<MarketingReportsPage>()));

        NavigateToUsersCommand = new RelayCommand(() =>
            LoadPage("Users", () => _services.GetRequiredService<UsersPage>()));

        NavigateToSettingsCommand = new RelayCommand(() =>
            LoadPage("Settings", () => _services.GetRequiredService<SettingsPage>()));

        BindingContext = this;

        // Load dashboard by default
        LoadPage("Dashboard", () => _services.GetRequiredService<CompanyDashboardPage>());
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

                        //System.Diagnostics.Debug.WriteLine($"Loaded page: {pageName}");
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine($"Page {pageName} has no content");
                    }
                }
                catch (Exception ex)
                {
                    //System.Diagnostics.Debug.WriteLine($"Error in MainThread: {ex.Message}");
                    DisplayAlert("Error", $"Failed to load {pageName}: {ex.Message}", "OK");
                }
            });
        }
        catch (Exception ex)
        {
            //System.Diagnostics.Debug.WriteLine($"Error loading page {pageName}: {ex.Message}");
            DisplayAlert("Error", $"Failed to load page: {ex.Message}", "OK");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
