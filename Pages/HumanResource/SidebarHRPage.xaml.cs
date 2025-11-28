using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using Headquartz.Pages.Dashboard;
using Headquartz.Pages;
using Headquartz.Pages.System;
using Headquartz.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Headquartz.Pages.HumanResource;

public partial class SidebarHRPage : ContentPage
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
    public RelayCommand NavigateToHRDashboardCommand { get; }
    public RelayCommand NavigateToEmployeeListCommand { get; }
    public RelayCommand NavigateToRecruitmentCommand { get; }
    public RelayCommand NavigateToPayrollCommand { get; }
    public RelayCommand NavigateToTrainingCommand { get; }
    public RelayCommand NavigateToHRPoliciesCommand { get; }
    public RelayCommand NavigateToHRReportsCommand { get; }
    public RelayCommand NavigateToUsersCommand { get; }
    public RelayCommand NavigateToSettingsCommand { get; }

    public SidebarHRPage(RoleService roleService, IServiceProvider services)
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

        NavigateToHRDashboardCommand = new RelayCommand(() =>
            LoadPage("HR Dashboard", () => _services.GetRequiredService<HRDashboardPage>()));

        NavigateToEmployeeListCommand = new RelayCommand(() =>              
            LoadPage("Employee List", () => _services.GetRequiredService<EmployeeListPage>()));

        NavigateToRecruitmentCommand = new RelayCommand(() =>              
            LoadPage("Recruitment", () => _services.GetRequiredService<RecruitmentPage>()));

        NavigateToPayrollCommand = new RelayCommand(() =>              
            LoadPage("Payroll", () => _services.GetRequiredService<PayrollPage>()));

        NavigateToTrainingCommand = new RelayCommand(() =>            
            LoadPage("Training", () => _services.GetRequiredService<TrainingPage>()));

        NavigateToHRPoliciesCommand = new RelayCommand(() =>
            LoadPage("HR Policies", () => _services.GetRequiredService<HRPoliciesPage>()));

       NavigateToHRReportsCommand = new RelayCommand(() =>            
            LoadPage("HR Reports", () => _services.GetRequiredService<HRReportsPage>()));

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
