using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Pages;
using Headquartz.Pages.CEO;
using Headquartz.Pages.Finance;
using Headquartz.Pages.HumanResource;
using Headquartz.Pages.Logistics;
using Headquartz.Pages.Marketing;
using Headquartz.Pages.Production;
using Headquartz.Pages.Sales;
using Headquartz.Pages.Warehouse;
using Headquartz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.PageModels
{
    public partial class LoginPageModel : ObservableObject
    {
        private readonly RoleService _roleService;
        private readonly INetworkService _networkService;
        private readonly ISimulationEngine _simulationEngine;

        public List<RolePermissions> Roles { get; }

        [ObservableProperty]
        private RolePermissions selectedRole;

        [ObservableProperty]
        private string ipAddress = "127.0.0.1";

        [ObservableProperty]
        private bool isConnected;

        [ObservableProperty]
        private string connectionStatus = "Not Connected";

        public IRelayCommand LoginCommand { get; }
        public IRelayCommand HostCommand { get; }
        public IRelayCommand JoinCommand { get; }
        public IRelayCommand AdminCommand { get; }

        public LoginPageModel(RoleService roleService, INetworkService networkService, ISimulationEngine simulationEngine)
        {
            _roleService = roleService;
            _networkService = networkService;
            _simulationEngine = simulationEngine;

            Roles = roleService.AvailableRoles;
            SelectedRole = Roles[0]; // default

            LoginCommand = new RelayCommand(OnLogin);
            HostCommand = new RelayCommand(OnHost);
            JoinCommand = new RelayCommand(OnJoin);
            AdminCommand = new RelayCommand(OnAdmin);
        }

        private void OnHost()
        {
            try 
            {
                _networkService.StartHost(9050);
                // Start the simulation engine as well since we are host
                if (_simulationEngine is SimulationEngine engine)
                {
                    engine.Start();
                }
                
                IsConnected = true;
                ConnectionStatus = "Hosting on Port 9050";
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
        }

        private void OnJoin()
        {
             try 
            {
                _networkService.Connect(IpAddress, 9050);
                IsConnected = true;
                ConnectionStatus = $"Connecting to {IpAddress}...";
            }
             catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
        }
        private async void OnAdmin()
        {
            await Shell.Current.GoToAsync($"//{nameof(AdministratorPage)}");

        }

        private async void OnLogin()
        {
            if (SelectedRole == null)
                return;

            _roleService.SetRole(SelectedRole);

            string targetPage = SelectedRole.Role switch
            {
                Role.CEO => nameof(SidebarCEOPage),
                Role.HRManager => nameof(SidebarHRPage),
                Role.FinanceManager => nameof(SidebarFinancePage),
                Role.SalesManager => nameof(SidebarSalesPage),
                Role.MarketingManager => nameof(SidebarMarketingPage),
                Role.ProductionManager => nameof(SidebarProductionPage),
                Role.WarehouseManager => nameof(SidebarWarehousePage),
                Role.LogisticsManager => nameof(SidebarLogisticsPage),
                _ => nameof(SidebarCEOPage)
            };

            await Shell.Current.GoToAsync($"//{targetPage}");
        }
    }
}
