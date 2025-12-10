using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models.Domain;
using Headquartz.Models.Warehouse;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Headquartz.PageModels.Warehouse
{
    public partial class WarehouseReportsPageModel : ObservableObject
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _updateTimer;

        [ObservableProperty]
        private int reportCount;

        [ObservableProperty]
        private string selectedPeriod;

        [ObservableProperty]
        private double overallEfficiency;

        [ObservableProperty]
        private string efficiencyColor;

        // Report Metrics
        [ObservableProperty]
        private double stockAccuracy;

        [ObservableProperty]
        private int pickingEfficiency;

        [ObservableProperty]
        private double spaceUtilization;

        [ObservableProperty]
        private double damageRate;

        [ObservableProperty]
        private double turnoverRate;

        [ObservableProperty]
        private int laborProductivity;

        // Custom Report Form
        [ObservableProperty]
        private string selectedReportType;

        [ObservableProperty]
        private string selectedTimePeriod;

        [ObservableProperty]
        private bool isPdfSelected;

        [ObservableProperty]
        private bool isExcelSelected;

        [ObservableProperty]
        private bool isPowerpointSelected;

        public ObservableCollection<string> ReportPeriods { get; } = new();
        public ObservableCollection<string> ReportTypes { get; } = new();
        public ObservableCollection<string> TimePeriods { get; } = new();
        public ObservableCollection<ReportDepartment> Departments { get; } = new();
        public ObservableCollection<RecentReport> RecentReports { get; } = new();

        public WarehouseReportsPageModel()
        {
            _gameState = GameState.Instance;

            // Initialize dropdowns
            ReportPeriods.Add("Last 7 Days");
            ReportPeriods.Add("Last 30 Days");
            ReportPeriods.Add("Last Quarter");
            ReportPeriods.Add("Last Year");

            ReportTypes.Add("Stock Accuracy Report");
            ReportTypes.Add("Picking Efficiency Report");
            ReportTypes.Add("Space Utilization Report");
            ReportTypes.Add("Damage Analysis Report");
            ReportTypes.Add("Inventory Turnover Report");
            ReportTypes.Add("Labor Productivity Report");
            ReportTypes.Add("Comprehensive Warehouse Report");

            TimePeriods.Add("Today");
            TimePeriods.Add("This Week");
            TimePeriods.Add("This Month");
            TimePeriods.Add("This Quarter");
            TimePeriods.Add("This Year");
            TimePeriods.Add("Custom Range");

            SelectedPeriod = "Last 30 Days";
            SelectedReportType = "Stock Accuracy Report";
            SelectedTimePeriod = "This Month";
            IsPdfSelected = true;

            InitializeDepartments();
            GenerateRecentReports();
            RefreshData();

            _updateTimer = new System.Timers.Timer(10000);
            _updateTimer.Elapsed += (s, e) => RefreshData();
            _updateTimer.Start();
        }

        private void InitializeDepartments()
        {
            Departments.Add(new ReportDepartment { Name = "Receiving", IsSelected = true });
            Departments.Add(new ReportDepartment { Name = "Storage", IsSelected = true });
            Departments.Add(new ReportDepartment { Name = "Picking", IsSelected = true });
            Departments.Add(new ReportDepartment { Name = "Packing", IsSelected = true });
            Departments.Add(new ReportDepartment { Name = "Shipping", IsSelected = true });
            Departments.Add(new ReportDepartment { Name = "Quality Control", IsSelected = false });
        }

        private void GenerateRecentReports()
        {
            var random = new Random();
            var reportTypes = new[]
            {
                new { Name = "Stock Accuracy Report", Icon = "📦", Color = "#4ECDC4" },
                new { Name = "Picking Efficiency Report", Icon = "⚡", Color = "#FF6B6B" },
                new { Name = "Space Utilization Report", Icon = "📐", Color = "#06D6A0" },
                new { Name = "Damage Analysis Report", Icon = "⚠️", Color = "#118AB2" },
                new { Name = "Labor Productivity Report", Icon = "👷", Color = "#9D4EDD" }
            };

            for (int i = 0; i < 5; i++)
            {
                var reportType = reportTypes[random.Next(reportTypes.Length)];
                RecentReports.Add(new RecentReport
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = reportType.Name,
                    Icon = reportType.Icon,
                    ReportColor = reportType.Color,
                    GeneratedDate = _gameState.CurrentGameDate.AddDays(-random.Next(1, 30))
                });
            }
        }

        private void RefreshData()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var random = new Random();

                ReportCount = 6;

                // Calculate metrics
                StockAccuracy = 0.96 + (random.NextDouble() * 0.04);
                PickingEfficiency = 25 + random.Next(-5, 10);
                SpaceUtilization = 0.65 + (random.NextDouble() * 0.2);
                DamageRate = 0.02 + (random.NextDouble() * 0.03);
                TurnoverRate = 3.5 + (random.NextDouble() * 1.5);
                LaborProductivity = 45 + random.Next(-10, 15);

                // Calculate overall efficiency
                OverallEfficiency = (StockAccuracy + (1 - DamageRate) + (SpaceUtilization * 1.2) +
                                    (Math.Min(PickingEfficiency / 30.0, 1.0))) / 4.0;

                EfficiencyColor = OverallEfficiency switch
                {
                    >= 0.9 => "#06D6A0",
                    >= 0.75 => "#FFD166",
                    >= 0.6 => "#F59E0B",
                    _ => "#EF4444"
                };
            });
        }

        [RelayCommand]
        private void ViewStockAccuracy()
        {
            System.Diagnostics.Debug.WriteLine("Opening Stock Accuracy Report...");
        }

        [RelayCommand]
        private void ViewPickingEfficiency()
        {
            System.Diagnostics.Debug.WriteLine("Opening Picking Efficiency Report...");
        }

        [RelayCommand]
        private void ViewSpaceUtilization()
        {
            System.Diagnostics.Debug.WriteLine("Opening Space Utilization Report...");
        }

        [RelayCommand]
        private void ViewDamageReport()
        {
            System.Diagnostics.Debug.WriteLine("Opening Damage Report...");
        }

        [RelayCommand]
        private void ViewTurnoverAnalysis()
        {
            System.Diagnostics.Debug.WriteLine("Opening Turnover Analysis Report...");
        }

        [RelayCommand]
        private void ViewLaborProductivity()
        {
            System.Diagnostics.Debug.WriteLine("Opening Labor Productivity Report...");
        }

        [RelayCommand]
        private void GenerateReport()
        {
            System.Diagnostics.Debug.WriteLine($"Generating report for period: {SelectedPeriod}");

            var random = new Random();
            var newReport = new RecentReport
            {
                Id = Guid.NewGuid().ToString(),
                Name = SelectedReportType,
                Icon = "📊",
                ReportColor = "#4ECDC4",
                GeneratedDate = _gameState.CurrentGameDate
            };

            RecentReports.Insert(0, newReport);

            // Keep only last 10 reports
            while (RecentReports.Count > 10)
            {
                RecentReports.RemoveAt(RecentReports.Count - 1);
            }
        }

        [RelayCommand]
        private void GenerateCustomReport()
        {
            if (string.IsNullOrWhiteSpace(SelectedReportType) || string.IsNullOrWhiteSpace(SelectedTimePeriod))
                return;

            var selectedDepartments = Departments.Where(d => d.IsSelected).Select(d => d.Name).ToList();

            System.Diagnostics.Debug.WriteLine($"Generating custom report: {SelectedReportType}");
            System.Diagnostics.Debug.WriteLine($"Period: {SelectedTimePeriod}");
            System.Diagnostics.Debug.WriteLine($"Departments: {string.Join(", ", selectedDepartments)}");
            System.Diagnostics.Debug.WriteLine($"Format: {(IsPdfSelected ? "PDF" : IsExcelSelected ? "Excel" : "PowerPoint")}");

            var newReport = new RecentReport
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"{SelectedReportType} - {SelectedTimePeriod}",
                Icon = "📊",
                ReportColor = "#4ECDC4",
                GeneratedDate = _gameState.CurrentGameDate
            };

            RecentReports.Insert(0, newReport);
        }

        [RelayCommand]
        private void OpenReport(string reportId)
        {
            var report = RecentReports.FirstOrDefault(r => r.Id == reportId);
            if (report != null)
            {
                System.Diagnostics.Debug.WriteLine($"Opening report: {report.Name}");
            }
        }

        [RelayCommand]
        private void ViewAllReports()
        {
            System.Diagnostics.Debug.WriteLine("Viewing all reports...");
        }

        [RelayCommand]
        private void ExportAll()
        {
            System.Diagnostics.Debug.WriteLine("Exporting all reports...");
        }

        [RelayCommand]
        private void ReportSettings()
        {
            System.Diagnostics.Debug.WriteLine("Opening report settings...");
        }

        [RelayCommand]
        private void DashboardView()
        {
            System.Diagnostics.Debug.WriteLine("Switching to dashboard view...");
        }

        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }
    }

}