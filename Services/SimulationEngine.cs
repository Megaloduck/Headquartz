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
        private Timer? _timer;
        private readonly object _lock = new();

        public event Action<GameState>? OnTicked;

        public TimeSpan TickRate { get; set; } = TimeSpan.FromSeconds(1);
        public bool IsRunning { get; private set; }

        public SimulationEngine(GameState state)
        {
            _state = state;
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
                // Advance simulation time
                _state.SimTime = _state.SimTime.Add(TickRate);

                // Update modules
                MarketModule.Update(_state);
                InventoryModule.Update(_state);
                HumanResourceModule.Update(_state);
                FinanceModule.Update(_state);


                // Notify UI on UI thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnTicked?.Invoke(_state);
                });
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}