using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Headquartz.Simulation
{
    public class TickEngine : IDisposable
    {
        private readonly object _lock = new object();
        private Timer? _timer;
        private int _intervalMs;
        private bool _running;

        public event Action<TickContext>? OnTick;

        public int CurrentTick { get; private set; }

        public TickEngine(int intervalMs = 1000)
        {
            _intervalMs = intervalMs;
            CurrentTick = 0;
        }
        public void Start()
        {
            lock (_lock)
            {
                if (_running) return;
                _timer = new Timer(TimerCallback, null, _intervalMs, _intervalMs);
                _running = true;
            }
        }


        public void Stop()
        {
            lock (_lock)
            {
                if (!_running) return;
                _timer?.Change(Timeout.Infinite, Timeout.Infinite);
                _timer?.Dispose();
                _timer = null;
                _running = false;
            }
        }
        private void TimerCallback(object? state)
        {
            // Prevent re-entrancy if tick processing takes longer than interval
            lock (_lock)
            {
                CurrentTick++;
                var ctx = new TickContext(CurrentTick, DateTime.UtcNow);
                try
                {
                    OnTick?.Invoke(ctx);
                }
                catch (Exception ex)
                {
                    // TODO: replace with your logging
                    Console.Error.WriteLine($"Tick {CurrentTick} threw: {ex}");
                }
            }
        }
        public void SetInterval(int ms)
        {
            lock (_lock)
            {
                _intervalMs = ms;
                if (_running && _timer != null)
                {
                    _timer.Change(_intervalMs, _intervalMs);
                }
            }
        }


        public void Dispose()
        {
            Stop();
        }
    }
}
