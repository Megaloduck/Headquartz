using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using Headquartz.Services;
using System.Collections.ObjectModel;

namespace Headquartz.PageModels
{
    public partial class OverviewPageModel : ObservableObject
    {
        private readonly ISimulationEngine _engine;
        private readonly GameState _state;
        private readonly ISaveService _saveService;
        private System.Timers.Timer _uiTimer;

        public OverviewPageModel(ISimulationEngine engine, GameState state, ISaveService saveService)
        {
            _engine = engine;
            _state = state;
            _saveService = saveService;

            // Subscribe to tick updates
            _engine.OnTicked += OnEngineTicked;

            // Subscribe to game events
            _state.OnGameEvent += OnGameEventReceived;

            // Initialize UI update timer (updates UI every second)
            _uiTimer = new System.Timers.Timer(1000);
            _uiTimer.Elapsed += (s, e) => UpdateBindings();
            _uiTimer.Start();

            // Initialize sample data
            InitializeSampleData();

            // Initial update
            UpdateBindings();
        }

        // Simulation Time Properties
        [ObservableProperty]
        private string _simTime = "2025-01-01";

        [ObservableProperty]
        private string _simulationStatus = "Paused";

        [ObservableProperty]
        private Color _simulationStatusColor = Colors.Orange;

        [ObservableProperty]
        private string _simulationPhase = "Planning Phase";

        [ObservableProperty]
        private double _timeProgress = 0.35;

        [ObservableProperty]
        private int _currentQuarter = 1;

        // Financial Properties
        [ObservableProperty]
        private string _cash = "$1,000,000.00";

        [ObservableProperty]
        private double _currentDemand = 1.0;

        [ObservableProperty]
        private int _totalStock = 0;

        // Speed Control Properties
        [ObservableProperty]
        private bool _isSpeed1x = true;

        [ObservableProperty]
        private bool _isSpeed2x = false;

        [ObservableProperty]
        private bool _isSpeed4x = false;

        [ObservableProperty]
        private bool _isSpeed8x = false;

        // Phase Indicators
        [ObservableProperty]
        private bool _isPlanningPhase = true;

        [ObservableProperty]
        private bool _isExecutionPhase = false;

        [ObservableProperty]
        private bool _isReviewPhase = false;

        // Events
        [ObservableProperty]
        private ObservableCollection<SimulationEvent> _upcomingEvents = new();

        [ObservableProperty]
        private bool _hasNoEvents = false;

        [ObservableProperty]
        private ObservableCollection<bool> _hasEventToday = new() { true, false, true, false, true, false, false };

        [ObservableProperty]
        private ObservableCollection<bool> _isToday = new() { true, false, false, false, false, false, false };

        // Milestones
        [ObservableProperty]
        private double _quarterProgress = 0.35;

        [ObservableProperty]
        private int _eventsCompleted = 12;

        [ObservableProperty]
        private int _decisionsMade = 45;

        [ObservableProperty]
        private int _simulationHours = 168;

        // Commands
        [RelayCommand]
        private void StartSimulation()
        {
            _engine.Start();
            SimulationStatus = "Running";
            SimulationStatusColor = Colors.Green;
        }

        [RelayCommand]
        private void StopSimulation()
        {
            _engine.Stop();
            SimulationStatus = "Paused";
            SimulationStatusColor = Colors.Orange;
        }

        [RelayCommand]
        private async Task SaveGame()
        {
            try
            {
                string path = FileNameForSave();
                await _saveService.SaveAsync(_state, path);

                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Game saved successfully!",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Failed to save game: {ex.Message}",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task LoadGame()
        {
            try
            {
                string path = FileNameForSave();
                var loaded = await _saveService.LoadAsync(path);

                if (loaded != null)
                {
                    // Merge loaded state
                    _state.CurrentGameDate = loaded.CurrentGameDate;
                    _state.CashBalance = loaded.CashBalance;
                    _state.GameDay = loaded.GameDay;
                    _state.GameWeek = loaded.GameWeek;
                    _state.GameMonth = loaded.GameMonth;
                    _state.GameYear = loaded.GameYear;

                    UpdateBindings();

                    await Application.Current.MainPage.DisplayAlert(
                        "Success",
                        "Game loaded successfully!",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Failed to load game: {ex.Message}",
                    "OK");
            }
        }

        [RelayCommand]
        private void SetSpeed1x()
        {
            _engine.SetSpeed(1.0);
            UpdateSpeedButtons(1);
        }

        [RelayCommand]
        private void SetSpeed2x()
        {
            _engine.SetSpeed(2.0);
            UpdateSpeedButtons(2);
        }

        [RelayCommand]
        private void SetSpeed4x()
        {
            _engine.SetSpeed(4.0);
            UpdateSpeedButtons(4);
        }

        [RelayCommand]
        private void SetSpeed8x()
        {
            _engine.SetSpeed(8.0);
            UpdateSpeedButtons(8);
        }

        [RelayCommand]
        private async Task ViewAllEvents()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Upcoming Events",
                "Full event calendar view coming soon!",
                "OK");
        }

        [RelayCommand]
        private async Task ViewMilestones()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Milestones",
                "Detailed milestone tracking coming soon!",
                "OK");
        }

        // Helper Methods
        private void OnEngineTicked(GameState state)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateBindings();
                EventsCompleted++;
                SimulationHours = state.GameDay * 24;
            });
        }

        private void OnGameEventReceived(GameEvent gameEvent)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Add event to upcoming events
                var simEvent = new SimulationEvent
                {
                    Title = GetEventTitle(gameEvent.Type),
                    Description = gameEvent.Message,
                    Date = gameEvent.Timestamp,
                    Time = gameEvent.Timestamp.ToString("HH:mm"),
                    EventColor = GetEventColor(gameEvent.Severity)
                };

                UpcomingEvents.Insert(0, simEvent);

                // Keep only last 10 events
                while (UpcomingEvents.Count > 10)
                {
                    UpcomingEvents.RemoveAt(UpcomingEvents.Count - 1);
                }

                HasNoEvents = UpcomingEvents.Count == 0;
            });
        }

        private void UpdateBindings()
        {
            SimTime = _state.CurrentGameDate.ToString("yyyy-MM-dd");
            Cash = $"${_state.CashBalance:N2}";
            TotalStock = _state.Inventory.Sum(i => i.Value.Quantity);

            // Update time progress (assume 30 days per quarter)
            int dayOfQuarter = (_state.GameDay % 90);
            TimeProgress = dayOfQuarter / 90.0;
            CurrentQuarter = ((_state.GameMonth - 1) / 3) + 1;
            QuarterProgress = TimeProgress;

            // Update phase based on day of month
            int dayOfMonth = _state.GameDay % 30;
            if (dayOfMonth < 10)
            {
                IsPlanningPhase = true;
                IsExecutionPhase = false;
                IsReviewPhase = false;
                SimulationPhase = "Planning Phase";
            }
            else if (dayOfMonth < 25)
            {
                IsPlanningPhase = false;
                IsExecutionPhase = true;
                IsReviewPhase = false;
                SimulationPhase = "Execution Phase";
            }
            else
            {
                IsPlanningPhase = false;
                IsExecutionPhase = false;
                IsReviewPhase = true;
                SimulationPhase = "Review Phase";
            }
        }

        private void UpdateSpeedButtons(int speed)
        {
            IsSpeed1x = speed == 1;
            IsSpeed2x = speed == 2;
            IsSpeed4x = speed == 4;
            IsSpeed8x = speed == 8;
        }

        private void InitializeSampleData()
        {
            // Add some sample events
            UpcomingEvents.Add(new SimulationEvent
            {
                Title = "Quarterly Review",
                Description = "Q1 2025 performance review meeting",
                Date = DateTime.Now.AddDays(2),
                Time = "14:00",
                EventColor = Color.FromArgb("#4CAF50")
            });

            UpcomingEvents.Add(new SimulationEvent
            {
                Title = "Payroll Processing",
                Description = "Monthly payroll scheduled",
                Date = DateTime.Now.AddDays(4),
                Time = "09:00",
                EventColor = Color.FromArgb("#2196F3")
            });

            UpcomingEvents.Add(new SimulationEvent
            {
                Title = "Supplier Meeting",
                Description = "Review supply chain contracts",
                Date = DateTime.Now.AddDays(6),
                Time = "11:30",
                EventColor = Color.FromArgb("#FF9800")
            });

            HasNoEvents = UpcomingEvents.Count == 0;
        }

        private string GetEventTitle(EventType type)
        {
            return type switch
            {
                EventType.NewSalesOrder => "New Sales Order",
                EventType.OrderShipped => "Order Shipped",
                EventType.ProductionCompleted => "Production Completed",
                EventType.MaterialShortage => "Material Shortage",
                EventType.LowCashFlow => "Low Cash Flow Warning",
                EventType.PayrollProcessed => "Payroll Processed",
                EventType.MarketChange => "Market Conditions Changed",
                EventType.MachineBreakdown => "Equipment Breakdown",
                EventType.MajorOrder => "Major Order Received",
                EventType.SupplierDelay => "Supplier Delay",
                EventType.QualityIssue => "Quality Issue Detected",
                EventType.MarketOpportunity => "Market Opportunity",
                EventType.BudgetReview => "Budget Review",
                EventType.FinancialReport => "Financial Report",
                _ => "System Event"
            };
        }

        private Color GetEventColor(EventSeverity severity)
        {
            return severity switch
            {
                EventSeverity.Info => Color.FromArgb("#4CAF50"),
                EventSeverity.Medium => Color.FromArgb("#FF9800"),
                EventSeverity.High => Color.FromArgb("#F44336"),
                _ => Color.FromArgb("#2196F3")
            };
        }

        private string FileNameForSave()
        {
            string file = "headquartz_save.json";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(folder, file);
        }
    }

    // Supporting Model
    public class SimulationEvent
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public Color EventColor { get; set; }
    }
}