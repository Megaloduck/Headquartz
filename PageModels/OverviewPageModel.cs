using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using Headquartz.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Headquartz.PageModels
{
    public partial class OverviewPageModel : ObservableObject
    {
        private readonly ISimulationEngine _engine;
        private readonly GameState _state;
        private readonly ISaveService _saveService;

        public OverviewPageModel (ISimulationEngine engine, GameState state, ISaveService saveService)
        {
            _engine = engine;
            _state = state;
            _saveService = saveService;

            // Subscribe to tick updates
            _engine.OnTicked += OnEngineTicked;
        }

        [ObservableProperty]
        private string _simTime = "-";

        [ObservableProperty]
        private string _cash = "-";

        [ObservableProperty]
        private double _currentDemand;

        [ObservableProperty]
        private int _totalStock;

        [RelayCommand]
        public void StartSimulation()
        {
            _engine.Start();
        }

        [RelayCommand]
        public void StopSimulation()
        {
            _engine.Stop();
        }

        [RelayCommand]
        public async Task SaveGame()
        {
            try
            {
                // Choose a path or preformat a filename
                string path = FileNameForSave();
                await _saveService.SaveAsync(_state, path);
            }
            catch (Exception ex)
            {
                // Handle error: you may want to show UI message
                System.Diagnostics.Debug.WriteLine($"Save error: {ex}");
            }
        }

        [RelayCommand]
        public async Task LoadGame()
        {
            try
            {
                string path = FileNameForSave(); // or open file picker
                var loaded = await _saveService.LoadAsync(path);
                if (loaded != null)
                {
                    // merge loaded state
                    _state.SimTime = loaded.SimTime;
                    _state.Company = loaded.Company;
                    _state.Market = loaded.Market;
                    _state.Warehouse = loaded.Warehouse;
                    _state.HumanResource = loaded.HumanResource;
                    _state.Finance = loaded.Finance;
                    // Then update UI
                    UpdateBindings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load error: {ex}");
            }
        }

        private void OnEngineTicked(GameState state)
        {
            // Update properties that UI binds to
            SimTime = state.SimTime.ToString("HH:mm:ss");
            Cash = state.Finance.Cash.ToString("F2");
            CurrentDemand = state.Market.CurrentDemand;
            TotalStock = state.Warehouse.Products?.Sum(p => p.Stock) ?? 0;
        }

        private void UpdateBindings()
        {
            SimTime = _state.SimTime.ToString("HH:mm:ss");
            Cash = _state.Finance.Cash.ToString("F2");
            CurrentDemand = _state.Market.CurrentDemand;
            TotalStock = _state.Warehouse.Products?.Sum(p => p.Stock) ?? 0;
        }

        private string FileNameForSave()
        {
            // For simplicity, save in local app data folder
            string file = "headquartz_save.json";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return System.IO.Path.Combine(folder, file);
        }
    }
}