using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using Headquartz.Models.Warehouse;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Headquartz.PageModels.Warehouse
{
    public partial class StorageAllocationPageModel : ObservableObject
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _updateTimer;

        [ObservableProperty]
        private int totalBins;

        [ObservableProperty]
        private int totalSpace;

        [ObservableProperty]
        private double spaceUtilization;

        [ObservableProperty]
        private string utilizationColor;

        [ObservableProperty]
        private int emptyBins;

        [ObservableProperty]
        private double avgBinOccupancy;

        [ObservableProperty]
        private double zoneAUsage;

        [ObservableProperty]
        private string zoneAColor;

        [ObservableProperty]
        private int zoneABins;

        [ObservableProperty]
        private double zoneBUsage;

        [ObservableProperty]
        private string zoneBColor;

        [ObservableProperty]
        private int zoneBBins;

        [ObservableProperty]
        private double zoneCUsage;

        [ObservableProperty]
        private string zoneCColor;

        [ObservableProperty]
        private int zoneCBins;

        [ObservableProperty]
        private double zoneDUsage;

        [ObservableProperty]
        private string zoneDColor;

        [ObservableProperty]
        private int zoneDBins;

        [ObservableProperty]
        private string binSearchText;

        [ObservableProperty]
        private string selectedZone;

        [ObservableProperty]
        private string selectedStatus;

        [ObservableProperty]
        private int showingBins;

        // New Bin Form Fields
        [ObservableProperty]
        private string newBinCode;

        [ObservableProperty]
        private string newBinZone;

        [ObservableProperty]
        private string newBinSize;

        [ObservableProperty]
        private string newBinWeightCapacity;

        [ObservableProperty]
        private string newBinType;

        public ObservableCollection<string> Zones { get; } = new();
        public ObservableCollection<string> BinStatuses { get; } = new();
        public ObservableCollection<string> ZoneOptions { get; } = new();
        public ObservableCollection<string> BinTypes { get; } = new();
        public ObservableCollection<StorageBin> Bins { get; } = new();
        public ObservableCollection<OptimizationSuggestion> OptimizationSuggestions { get; } = new();

        public StorageAllocationPageModel()
        {
            _gameState = GameState.Instance;

            // Initialize dropdowns
            Zones.Add("All Zones");
            Zones.Add("Zone A");
            Zones.Add("Zone B");
            Zones.Add("Zone C");
            Zones.Add("Zone D");

            BinStatuses.Add("All Status");
            BinStatuses.Add("Empty");
            BinStatuses.Add("Available");
            BinStatuses.Add("Full");

            ZoneOptions.Add("Zone A - Bulk Storage");
            ZoneOptions.Add("Zone B - Picking Zone");
            ZoneOptions.Add("Zone C - Cold Storage");
            ZoneOptions.Add("Zone D - Hazardous");

            BinTypes.Add("Standard Shelf");
            BinTypes.Add("Pallet Rack");
            BinTypes.Add("Cold Storage");
            BinTypes.Add("Hazmat Bin");
            BinTypes.Add("High Bay");

            SelectedZone = "All Zones";
            SelectedStatus = "All Status";
            NewBinZone = "Zone A - Bulk Storage";
            NewBinType = "Standard Shelf";

            InitializeStorageData();
            GenerateOptimizationSuggestions();

            _updateTimer = new System.Timers.Timer(5000);
            _updateTimer.Elapsed += (s, e) => RefreshData();
            _updateTimer.Start();

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BinSearchText) ||
                    e.PropertyName == nameof(SelectedZone) ||
                    e.PropertyName == nameof(SelectedStatus))
                {
                    FilterBins();
                }
            };
        }

        private void InitializeStorageData()
        {
            var random = new Random();

            // Generate bins for each zone
            for (int zone = 0; zone < 4; zone++)
            {
                char zoneLetter = (char)('A' + zone);
                int binsInZone = random.Next(8, 15);

                for (int i = 1; i <= binsInZone; i++)
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        var occupancy = random.NextDouble();
                        var status = occupancy switch
                        {
                            0 => "Empty",
                            < 0.5 => "Available",
                            _ => "Full"
                        };

                        Bins.Add(new StorageBin
                        {
                            Id = Guid.NewGuid().ToString(),
                            BinCode = $"{zoneLetter}-{i:D2}-{j:D2}",
                            Zone = $"Zone {zoneLetter}",
                            Occupancy = occupancy,
                            ProductCount = (int)(occupancy * 50),
                            StatusColor = occupancy switch
                            {
                                0 => "#95A5A6",
                                < 0.3 => "#06D6A0",
                                < 0.7 => "#FFD166",
                                _ => "#EF4444"
                            }
                        });
                    }
                }
            }

            RefreshData();
        }

        private void GenerateOptimizationSuggestions()
        {
            OptimizationSuggestions.Add(new OptimizationSuggestion
            {
                Id = Guid.NewGuid().ToString(),
                Icon = "🔄",
                Title = "Relocate Fast-Moving Items",
                Description = "Move frequently picked items closer to shipping area",
                Impact = 0.15,
                ImpactColor = "#06D6A0",
                Effort = "Medium"
            });

            OptimizationSuggestions.Add(new OptimizationSuggestion
            {
                Id = Guid.NewGuid().ToString(),
                Icon = "📦",
                Title = "Consolidate Partial Bins",
                Description = "Combine partially filled bins to free up space",
                Impact = 0.08,
                ImpactColor = "#FFD166",
                Effort = "Low"
            });

            OptimizationSuggestions.Add(new OptimizationSuggestion
            {
                Id = Guid.NewGuid().ToString(),
                Icon = "⚡",
                Title = "Implement ABC Analysis",
                Description = "Organize inventory by velocity for optimal placement",
                Impact = 0.22,
                ImpactColor = "#06D6A0",
                Effort = "High"
            });

            OptimizationSuggestions.Add(new OptimizationSuggestion
            {
                Id = Guid.NewGuid().ToString(),
                Icon = "🗄️",
                Title = "Vertical Space Optimization",
                Description = "Utilize upper storage levels for slow-moving items",
                Impact = 0.12,
                ImpactColor = "#4ECDC4",
                Effort = "Medium"
            });
        }

        private void RefreshData()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var random = new Random();

                // Overall metrics
                TotalBins = Bins.Count;
                TotalSpace = 5000; // sqm
                SpaceUtilization = Bins.Average(b => b.Occupancy);

                UtilizationColor = SpaceUtilization switch
                {
                    >= 0.9 => "#EF4444",
                    >= 0.75 => "#F59E0B",
                    >= 0.5 => "#10B981",
                    _ => "#3B82F6"
                };

                EmptyBins = Bins.Count(b => b.Occupancy == 0);
                AvgBinOccupancy = Bins.Average(b => b.Occupancy);

                // Zone metrics
                var zoneABins = Bins.Where(b => b.Zone == "Zone A").ToList();
                ZoneAUsage = zoneABins.Any() ? zoneABins.Average(b => b.Occupancy) : 0;
                ZoneAColor = ZoneAUsage >= 0.8 ? "#F59E0B" : "#10B981";
                ZoneABins = zoneABins.Count;

                var zoneBBins = Bins.Where(b => b.Zone == "Zone B").ToList();
                ZoneBUsage = zoneBBins.Any() ? zoneBBins.Average(b => b.Occupancy) : 0;
                ZoneBColor = ZoneBUsage >= 0.8 ? "#F59E0B" : "#10B981";
                ZoneBBins = zoneBBins.Count;

                var zoneCBins = Bins.Where(b => b.Zone == "Zone C").ToList();
                ZoneCUsage = zoneCBins.Any() ? zoneCBins.Average(b => b.Occupancy) : 0;
                ZoneCColor = ZoneCUsage >= 0.8 ? "#F59E0B" : "#10B981";
                ZoneCBins = zoneCBins.Count;

                var zoneDBins = Bins.Where(b => b.Zone == "Zone D").ToList();
                ZoneDUsage = zoneDBins.Any() ? zoneDBins.Average(b => b.Occupancy) : 0;
                ZoneDColor = ZoneDUsage >= 0.8 ? "#F59E0B" : "#10B981";
                ZoneDBins = zoneDBins.Count;

                FilterBins();
            });
        }

        private void FilterBins()
        {
            var filtered = Bins
                .Where(b =>
                {
                    bool matchesZone = SelectedZone == "All Zones" || b.Zone == SelectedZone;

                    bool matchesStatus = SelectedStatus == "All Status" ||
                        (SelectedStatus == "Empty" && b.Occupancy == 0) ||
                        (SelectedStatus == "Available" && b.Occupancy > 0 && b.Occupancy < 0.5) ||
                        (SelectedStatus == "Full" && b.Occupancy >= 0.5);

                    bool matchesSearch = string.IsNullOrWhiteSpace(BinSearchText) ||
                        b.BinCode.Contains(BinSearchText, StringComparison.OrdinalIgnoreCase);

                    return matchesZone && matchesStatus && matchesSearch;
                })
                .ToList();

            ShowingBins = filtered.Count;
        }

        [RelayCommand]
        private void ViewBin(string binId)
        {
            var bin = Bins.FirstOrDefault(b => b.Id == binId);
            if (bin != null)
            {
                System.Diagnostics.Debug.WriteLine($"Viewing bin: {bin.BinCode}");
            }
        }

        [RelayCommand]
        private void EditBin(string binId)
        {
            var bin = Bins.FirstOrDefault(b => b.Id == binId);
            if (bin != null)
            {
                System.Diagnostics.Debug.WriteLine($"Editing bin: {bin.BinCode}");
            }
        }

        [RelayCommand]
        private void ApplyOptimization(string optimizationId)
        {
            var optimization = OptimizationSuggestions.FirstOrDefault(o => o.Id == optimizationId);
            if (optimization != null)
            {
                System.Diagnostics.Debug.WriteLine($"Applying optimization: {optimization.Title}");
                // In real implementation, apply the optimization logic
                RefreshData();
            }
        }

        [RelayCommand]
        private void CreateBin()
        {
            if (string.IsNullOrWhiteSpace(NewBinCode) || string.IsNullOrWhiteSpace(NewBinZone))
                return;

            var zoneLetter = NewBinZone.Substring(5, 1);
            var random = new Random();

            var newBin = new StorageBin
            {
                Id = Guid.NewGuid().ToString(),
                BinCode = NewBinCode,
                Zone = $"Zone {zoneLetter}",
                Occupancy = 0,
                ProductCount = 0,
                StatusColor = "#95A5A6"
            };

            Bins.Add(newBin);

            // Clear form
            NewBinCode = "";
            NewBinSize = "";
            NewBinWeightCapacity = "";

            RefreshData();
        }

        [RelayCommand]
        private void SpaceReport() => System.Diagnostics.Debug.WriteLine("Generating space report...");

        [RelayCommand]
        private void Reorganize() => System.Diagnostics.Debug.WriteLine("Starting reorganization...");

        [RelayCommand]
        private void ExportLayout() => System.Diagnostics.Debug.WriteLine("Exporting layout...");

        [RelayCommand]
        private void ThreeDView() => System.Diagnostics.Debug.WriteLine("Opening 3D view...");

        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }
    }   
}