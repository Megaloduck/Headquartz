using Headquartz.Models;
using Headquartz.Models.Production;
using Headquartz.Models.Sales;
using System.Diagnostics;

namespace Headquartz.Services
{
    /// <summary>
    /// Central tick manager coordinating all simulation subsystems
    /// Similar to MonsoonSIM's simulation controller
    /// </summary>
    public class TickManager : IDisposable
    {
        private readonly ISimulationEngine _engine;
        private readonly GameState _gameState;
        private readonly List<ITickable> _tickableModules;

        // Performance tracking
        private readonly Stopwatch _performanceWatch;
        private long _totalTickTime;
        private int _tickCount;

        // Subsystem processors
        private readonly ProductionProcessor _productionProcessor;
        private readonly FinanceProcessor _financeProcessor;
        private readonly HRProcessor _hrProcessor;
        private readonly LogisticsProcessor _logisticsProcessor;
        private readonly MarketProcessor _marketProcessor;

        public TickManager(ISimulationEngine engine, GameState gameState)
        {
            _engine = engine;
            _gameState = gameState;
            _tickableModules = new List<ITickable>();
            _performanceWatch = new Stopwatch();

            // Initialize subsystem processors
            _productionProcessor = new ProductionProcessor(gameState);
            _financeProcessor = new FinanceProcessor(gameState);
            _hrProcessor = new HRProcessor(gameState);
            _logisticsProcessor = new LogisticsProcessor(gameState);
            _marketProcessor = new MarketProcessor(gameState);

            // Register all processors
            RegisterTickable(_productionProcessor);
            RegisterTickable(_financeProcessor);
            RegisterTickable(_hrProcessor);
            RegisterTickable(_logisticsProcessor);
            RegisterTickable(_marketProcessor);

            // Subscribe to engine events
            _engine.OnDayTicked += OnDayTicked;
            _engine.OnWeekTicked += OnWeekTicked;
            _engine.OnMonthTicked += OnMonthTicked;
            _engine.OnQuarterTicked += OnQuarterTicked;
            _engine.OnYearTicked += OnYearTicked;

            Debug.WriteLine("[TickManager] Initialized with 5 subsystems");
        }

        public void RegisterTickable(ITickable tickable)
        {
            if (tickable != null && !_tickableModules.Contains(tickable))
            {
                _tickableModules.Add(tickable);
            }
        }

        private void OnDayTicked(GameState state)
        {
            _performanceWatch.Restart();

            try
            {
                // Process all tickable modules
                foreach (var module in _tickableModules)
                {
                    module.OnDayTick(state);
                }

                _tickCount++;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TickManager] ERROR during daily tick: {ex.Message}");
            }

            _performanceWatch.Stop();
            _totalTickTime += _performanceWatch.ElapsedMilliseconds;

            // Log performance every 100 ticks
            if (_tickCount % 100 == 0)
            {
                var avgTickTime = _totalTickTime / (double)_tickCount;
                Debug.WriteLine($"[TickManager] Performance: Avg tick time {avgTickTime:F2}ms over {_tickCount} ticks");
            }
        }

        private void OnWeekTicked(GameState state)
        {
            foreach (var module in _tickableModules)
            {
                module.OnWeekTick(state);
            }
            Debug.WriteLine($"[TickManager] Week {state.GameWeek} processed by {_tickableModules.Count} modules");
        }

        private void OnMonthTicked(GameState state)
        {
            foreach (var module in _tickableModules)
            {
                module.OnMonthTick(state);
            }
            Debug.WriteLine($"[TickManager] Month {state.GameMonth} processed");
        }

        private void OnQuarterTicked(GameState state)
        {
            foreach (var module in _tickableModules)
            {
                module.OnQuarterTick(state);
            }
            Debug.WriteLine($"[TickManager] Quarter {state.GameQuarter} processed");
        }

        private void OnYearTicked(GameState state)
        {
            foreach (var module in _tickableModules)
            {
                module.OnYearTick(state);
            }
            Debug.WriteLine($"[TickManager] Year {state.GameYear} processed");
        }

        public TickPerformanceReport GetPerformanceReport()
        {
            return new TickPerformanceReport
            {
                TotalTicks = _tickCount,
                TotalTimeMs = _totalTickTime,
                AverageTickTimeMs = _tickCount > 0 ? _totalTickTime / (double)_tickCount : 0,
                RegisteredModules = _tickableModules.Count,
                CurrentDay = _gameState.GameDay,
                IsRunning = _engine.IsRunning
            };
        }

        public void Dispose()
        {
            // Unsubscribe from events
            _engine.OnDayTicked -= OnDayTicked;
            _engine.OnWeekTicked -= OnWeekTicked;
            _engine.OnMonthTicked -= OnMonthTicked;
            _engine.OnQuarterTicked -= OnQuarterTicked;
            _engine.OnYearTicked -= OnYearTicked;

            Debug.WriteLine("[TickManager] Disposed");
        }
    }

    /// <summary>
    /// Interface for modules that process ticks
    /// </summary>
    public interface ITickable
    {
        void OnDayTick(GameState state);
        void OnWeekTick(GameState state);
        void OnMonthTick(GameState state);
        void OnQuarterTick(GameState state);
        void OnYearTick(GameState state);
    }

    public class TickPerformanceReport
    {
        public int TotalTicks { get; set; }
        public long TotalTimeMs { get; set; }
        public double AverageTickTimeMs { get; set; }
        public int RegisteredModules { get; set; }
        public int CurrentDay { get; set; }
        public bool IsRunning { get; set; }

        public override string ToString()
        {
            return $"Tick Performance:\n" +
                   $"  Total Ticks: {TotalTicks}\n" +
                   $"  Avg Time: {AverageTickTimeMs:F2}ms\n" +
                   $"  Modules: {RegisteredModules}\n" +
                   $"  Day: {CurrentDay}\n" +
                   $"  Status: {(IsRunning ? "Running" : "Paused")}";
        }
    }

    #region Subsystem Processors

    public class ProductionProcessor : ITickable
    {
        private readonly GameState _state;

        public ProductionProcessor(GameState state)
        {
            _state = state;
        }

        public void OnDayTick(GameState state)
        {
            // Daily production progress handled in GameState.ProcessProduction()
        }

        public void OnWeekTick(GameState state)
        {
            // Weekly maintenance checks
            foreach (var workOrder in state.ActiveWorkOrders)
            {
                if (workOrder.Status == WorkOrderStatus.InProgress)
                {
                    // Check for delays
                    var daysSinceStart = (state.CurrentGameDate - workOrder.StartDate).Days;
                    if (daysSinceStart > 14 && workOrder.ProgressPercentage < 50)
                    {
                        Debug.WriteLine($"[Production] Work order {workOrder.Id} behind schedule");
                    }
                }
            }
        }

        public void OnMonthTick(GameState state)
        {
            // Monthly production reports
            var completed = state.ActiveWorkOrders.Count(w => w.Status == WorkOrderStatus.Completed);
            Debug.WriteLine($"[Production] Monthly summary: {completed} orders completed");
        }

        public void OnQuarterTick(GameState state) { }
        public void OnYearTick(GameState state) { }
    }

    public class FinanceProcessor : ITickable
    {
        private readonly GameState _state;

        public FinanceProcessor(GameState state)
        {
            _state = state;
        }

        public void OnDayTick(GameState state)
        {
            // Daily interest on loans (if implemented)
        }

        public void OnWeekTick(GameState state)
        {
            // Weekly financial health check
            var weeklyBurnRate = state.MonthlyExpenses / 4;
            if (state.CashBalance < weeklyBurnRate * 2)
            {
                Debug.WriteLine("[Finance] WARNING: Low cash runway");
            }
        }

        public void OnMonthTick(GameState state)
        {
            // Monthly financial close
            Debug.WriteLine($"[Finance] Month closed: P&L = ${state.NetProfit:N0}");
        }

        public void OnQuarterTick(GameState state)
        {
            Debug.WriteLine($"[Finance] Quarterly P&L: ${state.QuarterlyProfit:N0}");
        }

        public void OnYearTick(GameState state)
        {
            Debug.WriteLine($"[Finance] Annual P&L: ${state.YearlyProfit:N0}");
        }
    }

    public class HRProcessor : ITickable
    {
        private readonly GameState _state;

        public HRProcessor(GameState state)
        {
            _state = state;
        }

        public void OnDayTick(GameState state)
        {
            // Daily attendance and productivity
        }

        public void OnWeekTick(GameState state)
        {
            // Weekly satisfaction surveys
            var avgSatisfaction = state.Employees.Any()
                ? state.Employees.Average(e => e.SatisfactionLevel)
                : 75;

            if (avgSatisfaction < 60)
            {
                Debug.WriteLine("[HR] WARNING: Low employee satisfaction");
            }
        }

        public void OnMonthTick(GameState state)
        {
            // Monthly performance reviews
            Debug.WriteLine($"[HR] Workforce: {state.Employees.Count} employees");
        }

        public void OnQuarterTick(GameState state) { }
        public void OnYearTick(GameState state)
        {
            Debug.WriteLine("[HR] Annual reviews and raises processed");
        }
    }

    public class LogisticsProcessor : ITickable
    {
        private readonly GameState _state;

        public LogisticsProcessor(GameState state)
        {
            _state = state;
        }

        public void OnDayTick(GameState state)
        {
            // Daily shipment processing handled in GameState
        }

        public void OnWeekTick(GameState state)
        {
            // Weekly delivery performance
            var shipped = state.ActiveSalesOrders.Count(o => o.Status == OrderStatus.Shipped);
            Debug.WriteLine($"[Logistics] Weekly shipments: {shipped}");
        }

        public void OnMonthTick(GameState state) { }
        public void OnQuarterTick(GameState state) { }
        public void OnYearTick(GameState state) { }
    }

    public class MarketProcessor : ITickable
    {
        private readonly GameState _state;

        public MarketProcessor(GameState state)
        {
            _state = state;
        }

        public void OnDayTick(GameState state)
        {
            // Daily market fluctuations (small)
        }

        public void OnWeekTick(GameState state)
        {
            // Weekly market analysis handled in GameState
        }

        public void OnMonthTick(GameState state)
        {
            Debug.WriteLine($"[Market] Market share: {state.MarketShare}%");
        }

        public void OnQuarterTick(GameState state)
        {
            // Quarterly market strategy review
            Debug.WriteLine("[Market] Quarterly market analysis complete");
        }

        public void OnYearTick(GameState state)
        {
            // Annual market positioning
            Debug.WriteLine("[Market] Annual market report generated");
        }
    }

    #endregion
}