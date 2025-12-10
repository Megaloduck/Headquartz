using Headquartz.Models;
using System.Timers;

namespace Headquartz.Services
{
    public class SimulationEngine : ISimulationEngine
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _timer;
        private double _speedMultiplier = 1.0;
        private bool _isRunning = false;

        public event Action<GameState> OnTicked;
        public bool IsRunning => _isRunning;
        public double CurrentSpeed => _speedMultiplier;

        public SimulationEngine(GameState gameState)
        {
            _gameState = gameState;

            // Initialize timer (tick every second, adjusted by speed)
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _timer.Stop();
            }
        }

        public void SetSpeed(double multiplier)
        {
            _speedMultiplier = Math.Max(0.1, Math.Min(10.0, multiplier));

            // Adjust timer interval based on speed
            // Base interval is 1000ms, so at 2x speed it should be 500ms
            _timer.Interval = 1000 / _speedMultiplier;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!_isRunning) return;

            try
            {
                // Advance game time
                _gameState.AdvanceGameTime();

                // Notify listeners
                OnTicked?.Invoke(_gameState);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Simulation tick error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}