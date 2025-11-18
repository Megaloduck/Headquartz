using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Headquartz.Modules;
using Headquartz.Models;

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
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TickRate);
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


        private void TimerCallback(object? state)
        {
            lock (_lock)
            {
                // advance simulation time
                _state.SimTime = _state.SimTime.AddSeconds(TickRate.TotalSeconds);


                // tick modules
                MarketModule.Update(_state);


                // other modules: ProcurementModule.Update(_state) ...


                // notify subscribers (UI)
                OnTicked?.Invoke(_state);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
