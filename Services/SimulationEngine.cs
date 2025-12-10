using Headquartz.Models;
using Headquartz.Modules;
using Microsoft.Maui.Dispatching;
using System;
using System.Reflection;
using System.Threading;

namespace Headquartz.Services
{
    public class SimulationEngine : ISimulationEngine, IDisposable
    {
        private readonly GameState _state;
        private readonly INetworkService _networkService;
        private Timer? _timer;
        private readonly object _lock = new();

        public event Action<GameState>? OnTicked;
        public GameState State => _state;

        public TimeSpan TickRate { get; set; } = TimeSpan.FromSeconds(1); 
        public bool IsRunning { get; private set; }

        public SimulationEngine(GameState state, INetworkService networkService)
        {
            _state = state;
            _networkService = networkService;
            
            // Listen for state updates if we are a client
            _networkService.OnGameStateReceived += HandleNetworkStateUpdate;
        }

        public void Start()
        {
            if (IsRunning) return;

            _timer = new Timer(Tick, null, TimeSpan.Zero, TickRate);
            IsRunning = true;
        }

        public void Stop()
        {
            if (!IsRunning) return;

            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _timer?.Dispose();
            _timer = null;

            IsRunning = false;
        }

        private void Tick(object? _)
        {
            lock (_lock)
            {
                // 1. Process Network Events (Send/Receive)
                _networkService.PollEvents();

                // 2. If we are HOST, run the simulation
                if (_networkService.IsHost)
                {
                    // Advance simulation time (1 Tick = 1 Day for simplicity in this demo)
                    _state.AdvanceGameTime();

                    // Update modules (if they have additional logic outside GameState internal logic)
                    MarketModule.Update(_state);
                    WarehouseModule.Update(_state);
                    HumanResourceModule.Update(_state);
                    FinanceModule.Update(_state);

                    // Broadcast new state to clients
                    _networkService.BroadcastGameState(_state);
                }
                
                // 3. Notify UI (Both Host and Client need to update UI)
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnTicked?.Invoke(_state);
                });
            }
        }

        private void HandleNetworkStateUpdate(GameState newState)
        {
            lock (_lock)
            {
                // We are a client, so we overwrite our local state with the server's state
                
                _state.CurrentGameDate = newState.CurrentGameDate;
                _state.GameDay = newState.GameDay;
                _state.GameMonth = newState.GameMonth;
                _state.GameYear = newState.GameYear;
                
                _state.CashBalance = newState.CashBalance;
                _state.MonthlyRevenue = newState.MonthlyRevenue;
                _state.MonthlyExpenses = newState.MonthlyExpenses;
                
                // Note: Collections (Inventory, Orders) are harder to sync via simple JSON assignment 
                // without full deep copy logic, but for now this covers the main dashboard stats.
            }
        }

        public void Dispose()
        {
            _networkService.OnGameStateReceived -= HandleNetworkStateUpdate;
            Stop();
        }
    }
}