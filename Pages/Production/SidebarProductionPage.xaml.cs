using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using Headquartz.Pages.Dashboard;
using Headquartz.Pages.Sales;
using Headquartz.Pages.System;
using Headquartz.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Headquartz.Pages.Production;

public partial class SidebarProductionPage : ContentPage
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
    public RelayCommand NavigateToProductionDashboardCommand { get; }
    public RelayCommand NavigateToWorkOrdersCommand { get; }
    public RelayCommand NavigateToFactoryScheduleCommand { get; }
    public RelayCommand NavigateToRawMaterialsCommand { get; }
    public RelayCommand NavigateToMachinesCommand { get; }
    public RelayCommand NavigateToProductionQualityCommand { get; }
    public RelayCommand NavigateToProductionReportsCommand { get; }
    public RelayCommand NavigateToUsersCommand { get; }
    public RelayCommand NavigateToSettingsCommand { get; }

    public SidebarProductionPage(RoleService roleService, IServiceProvider services)
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

        NavigateToProductionDashboardCommand = new RelayCommand(() =>
            LoadPage("Production Dashboard", () => _services.GetRequiredService<ProductionDashboardPage>()));

        NavigateToWorkOrdersCommand = new RelayCommand(() =>
            LoadPage("Work Orders", () => _services.GetRequiredService<WorkOrdersPage>()));

        NavigateToFactoryScheduleCommand = new RelayCommand(() =>
            LoadPage("Factory Schedule", () => _services.GetRequiredService<FactorySchedulePage>()));

        NavigateToRawMaterialsCommand = new RelayCommand(() =>
            LoadPage("Raw Materials", () => _services.GetRequiredService<RawMaterialsPage>()));

        NavigateToMachinesCommand = new RelayCommand(() =>
            LoadPage("Machines", () => _services.GetRequiredService<MachinesPage>()));

        NavigateToProductionQualityCommand = new RelayCommand(() =>
            LoadPage("Production Quality", () => _services.GetRequiredService<ProductionQualityPage>()));

        NavigateToProductionReportsCommand = new RelayCommand(() =>
            LoadPage("Production Reports", () => _services.GetRequiredService<ProductionReportsPage>()));

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
