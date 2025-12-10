using Headquartz.Models;
using System.Timers;

namespace Headquartz.Services
{
    /// <summary>
    /// Enhanced Simulation Engine with MonsoonSIM-like tick mechanism
    /// Features:
    /// - Multiple tick speeds (Pause, 1x, 2x, 4x, 8x, 16x)
    /// - Daily, Weekly, Monthly, Quarterly, and Yearly processing
    /// - Event-driven architecture
    /// - Simulation phases (Planning, Execution, Review)
    /// </summary>
    public class SimulationEngine : ISimulationEngine, IDisposable
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _timer;
        private double _speedMultiplier = 1.0;
        private bool _isRunning = false;
        private int _tickCounter = 0;
        private DateTime _lastTickTime;

        // Tick configuration
        private const int BASE_TICK_INTERVAL_MS = 1000; // 1 second = 1 game day at 1x speed
        private const int TICKS_PER_WEEK = 7;
        private const int TICKS_PER_MONTH = 30;
        private const int TICKS_PER_QUARTER = 90;
        private const int TICKS_PER_YEAR = 360;

        // Events
        public event Action<GameState> OnTicked;
        public event Action<GameState> OnDayTicked;
        public event Action<GameState> OnWeekTicked;
        public event Action<GameState> OnMonthTicked;
        public event Action<GameState> OnQuarterTicked;
        public event Action<GameState> OnYearTicked;
        public event Action<SimulationPhase> OnPhaseChanged;

        // Properties
        public bool IsRunning => _isRunning;
        public double CurrentSpeed => _speedMultiplier;
        public SimulationPhase CurrentPhase { get; private set; } = SimulationPhase.Planning;
        public int TicksProcessed => _tickCounter;
        public DateTime LastTickTime => _lastTickTime;

        public SimulationEngine(GameState gameState)
        {
            _gameState = gameState;
            _lastTickTime = DateTime.Now;

            // Initialize timer
            _timer = new System.Timers.Timer(BASE_TICK_INTERVAL_MS);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;

            System.Diagnostics.Debug.WriteLine("[SimEngine] Simulation Engine initialized");
        }

        #region Control Methods

        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _lastTickTime = DateTime.Now;
                _timer.Start();
                System.Diagnostics.Debug.WriteLine($"[SimEngine] Simulation STARTED at speed {_speedMultiplier}x");
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _timer.Stop();
                System.Diagnostics.Debug.WriteLine("[SimEngine] Simulation PAUSED");
            }
        }

        public void SetSpeed(double multiplier)
        {
            // Clamp speed between 0.1x and 16x
            _speedMultiplier = Math.Max(0.1, Math.Min(16.0, multiplier));

            // Adjust timer interval: faster speed = shorter interval
            double newInterval = BASE_TICK_INTERVAL_MS / _speedMultiplier;
            _timer.Interval = Math.Max(50, newInterval); // Minimum 50ms to prevent overload

            System.Diagnostics.Debug.WriteLine($"[SimEngine] Speed changed to {_speedMultiplier}x (interval: {_timer.Interval}ms)");
        }

        public void Reset()
        {
            Stop();
            _tickCounter = 0;
            _gameState.GameDay = 1;
            _gameState.GameWeek = 1;
            _gameState.GameMonth = 1;
            _gameState.GameYear = 1;
            _gameState.CurrentGameDate = new DateTime(2025, 1, 1);
            System.Diagnostics.Debug.WriteLine("[SimEngine] Simulation RESET");
        }

        #endregion

        #region Tick Processing

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!_isRunning) return;

            try
            {
                _lastTickTime = DateTime.Now;
                ProcessTick();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SimEngine] ERROR during tick: {ex.Message}");
                Stop(); // Stop simulation on critical error
            }
        }

        private void ProcessTick()
        {
            _tickCounter++;

            // Advance game time
            _gameState.AdvanceGameTime();

            // Update simulation phase
            UpdateSimulationPhase();

            // Fire daily tick event
            OnDayTicked?.Invoke(_gameState);
            OnTicked?.Invoke(_gameState);

            // Process periodic events
            CheckPeriodicEvents();

            // Log tick for debugging
            if (_tickCounter % 30 == 0) // Log every 30 days
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[SimEngine] Day {_gameState.GameDay} | " +
                    $"Date: {_gameState.CurrentGameDate:yyyy-MM-dd} | " +
                    $"Cash: ${_gameState.CashBalance:N0} | " +
                    $"Phase: {CurrentPhase}");
            }
        }

        private void CheckPeriodicEvents()
        {
            int day = _gameState.GameDay;

            // Weekly events (every 7 days)
            if (day % TICKS_PER_WEEK == 0)
            {
                OnWeekTicked?.Invoke(_gameState);
                System.Diagnostics.Debug.WriteLine($"[SimEngine] Week {_gameState.GameWeek} completed");
            }

            // Monthly events (every 30 days)
            if (day % TICKS_PER_MONTH == 0)
            {
                OnMonthTicked?.Invoke(_gameState);
                System.Diagnostics.Debug.WriteLine($"[SimEngine] Month {_gameState.GameMonth} completed");
            }

            // Quarterly events (every 90 days)
            if (day % TICKS_PER_QUARTER == 0)
            {
                int quarter = (day / TICKS_PER_QUARTER);
                OnQuarterTicked?.Invoke(_gameState);
                System.Diagnostics.Debug.WriteLine($"[SimEngine] Quarter {quarter} completed");
            }

            // Yearly events (every 360 days)
            if (day % TICKS_PER_YEAR == 0)
            {
                OnYearTicked?.Invoke(_gameState);
                System.Diagnostics.Debug.WriteLine($"[SimEngine] Year {_gameState.GameYear} completed");
            }
        }

        #endregion

        #region Simulation Phases

        private void UpdateSimulationPhase()
        {
            // Determine phase based on day of month
            int dayOfMonth = _gameState.GameDay % TICKS_PER_MONTH;
            SimulationPhase newPhase;

            if (dayOfMonth < 10) // Days 1-10: Planning
            {
                newPhase = SimulationPhase.Planning;
            }
            else if (dayOfMonth < 25) // Days 11-25: Execution
            {
                newPhase = SimulationPhase.Execution;
            }
            else // Days 26-30: Review
            {
                newPhase = SimulationPhase.Review;
            }

            // Trigger phase change event
            if (newPhase != CurrentPhase)
            {
                CurrentPhase = newPhase;
                OnPhaseChanged?.Invoke(newPhase);
                System.Diagnostics.Debug.WriteLine($"[SimEngine] Phase changed to: {newPhase}");
            }
        }

        #endregion

        #region Statistics & Diagnostics

        public SimulationStatistics GetStatistics()
        {
            return new SimulationStatistics
            {
                TicksProcessed = _tickCounter,
                CurrentDay = _gameState.GameDay,
                CurrentWeek = _gameState.GameWeek,
                CurrentMonth = _gameState.GameMonth,
                CurrentYear = _gameState.GameYear,
                CurrentDate = _gameState.CurrentGameDate,
                SimulationSpeed = _speedMultiplier,
                IsRunning = _isRunning,
                CurrentPhase = CurrentPhase,
                LastTickTime = _lastTickTime,
                CashBalance = _gameState.CashBalance,
                TotalInventoryItems = _gameState.Inventory.Count,
                ActiveOrders = _gameState.ActiveSalesOrders.Count,
                ActiveWorkOrders = _gameState.ActiveWorkOrders.Count,
                TotalEmployees = _gameState.Employees.Count
            };
        }

        #endregion

        #region Disposal

        public void Dispose()
        {
            Stop();
            _timer?.Dispose();
            System.Diagnostics.Debug.WriteLine("[SimEngine] Simulation Engine disposed");
        }

        #endregion
    }

    /// <summary>
    /// Simulation phases similar to MonsoonSIM
    /// </summary>
    public enum SimulationPhase
    {
        Planning,   // Strategic planning period
        Execution,  // Operations and production
        Review      // Performance review and analysis
    }

    /// <summary>
    /// Statistics for monitoring simulation performance
    /// </summary>
    public class SimulationStatistics
    {
        public int TicksProcessed { get; set; }
        public int CurrentDay { get; set; }
        public int CurrentWeek { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
        public DateTime CurrentDate { get; set; }
        public double SimulationSpeed { get; set; }
        public bool IsRunning { get; set; }
        public SimulationPhase CurrentPhase { get; set; }
        public DateTime LastTickTime { get; set; }
        public decimal CashBalance { get; set; }
        public int TotalInventoryItems { get; set; }
        public int ActiveOrders { get; set; }
        public int ActiveWorkOrders { get; set; }
        public int TotalEmployees { get; set; }

        public override string ToString()
        {
            return $"Simulation Stats:\n" +
                   $"  Day: {CurrentDay} ({CurrentDate:yyyy-MM-dd})\n" +
                   $"  Phase: {CurrentPhase}\n" +
                   $"  Speed: {SimulationSpeed}x\n" +
                   $"  Cash: ${CashBalance:N0}\n" +
                   $"  Orders: {ActiveOrders}\n" +
                   $"  Employees: {TotalEmployees}";
        }
    }
}