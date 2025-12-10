using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models.Domain;
using Headquartz.Models.Warehouse;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Headquartz.PageModels.Warehouse
{
    public partial class InventoryPageModel : ObservableObject
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _updateTimer;

        [ObservableProperty]
        private int totalItems;

        [ObservableProperty]
        private decimal totalInventoryValue;

        [ObservableProperty]
        private int averageStockLevel;

        [ObservableProperty]
        private double turnoverRate;

        [ObservableProperty]
        private double inventoryAccuracy;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private string selectedCategory;

        [ObservableProperty]
        private int showingCount;

        public ObservableCollection<string> Categories { get; } = new();
        public ObservableCollection<InventoryItemPageModel> InventoryItems { get; } = new();
        public ObservableCollection<ReorderSuggestion> ReorderSuggestions { get; } = new();
        public ObservableCollection<CategoryDistribution> CategoryDistribution { get; } = new();

        public InventoryPageModel()
        {
            _gameState = GameState.Instance;

            Categories.Add("All Categories");
            Categories.Add("Raw Materials");
            Categories.Add("Components");
            Categories.Add("Finished Goods");
            Categories.Add("Consumables");

            SelectedCategory = "All Categories";
            _gameState.OnGameEvent += HandleGameEvent;
            InitializeInventoryData();

            _updateTimer = new System.Timers.Timer(3000);
            _updateTimer.Elapsed += (s, e) => RefreshData();
            _updateTimer.Start();

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SearchText) || e.PropertyName == nameof(SelectedCategory))
                    FilterInventory();
            };
        }

        private void InitializeInventoryData()
        {
            if (_gameState.Inventory.Count == 0)
                CreateInitialInventory();
            RefreshData();
        }

        private void CreateInitialInventory()
        {
            var initialProducts = new[]
            {
                new { Id = "RM-001", Name = "Steel Sheets", Category = "Raw Materials", Qty = 500, Cost = 25m, Reorder = 200 },
                new { Id = "RM-002", Name = "Aluminum Bars", Category = "Raw Materials", Qty = 350, Cost = 30m, Reorder = 150 },
                new { Id = "RM-003", Name = "Plastic Pellets", Category = "Raw Materials", Qty = 800, Cost = 15m, Reorder = 300 },
                new { Id = "COMP-001", Name = "Circuit Boards", Category = "Components", Qty = 200, Cost = 50m, Reorder = 100 },
                new { Id = "COMP-002", Name = "Motors", Category = "Components", Qty = 150, Cost = 75m, Reorder = 75 },
                new { Id = "COMP-003", Name = "Sensors", Category = "Components", Qty = 300, Cost = 40m, Reorder = 150 },
                new { Id = "FG-001", Name = "Product Alpha", Category = "Finished Goods", Qty = 100, Cost = 200m, Reorder = 50 },
                new { Id = "FG-002", Name = "Product Beta", Category = "Finished Goods", Qty = 75, Cost = 250m, Reorder = 40 },
                new { Id = "FG-003", Name = "Product Gamma", Category = "Finished Goods", Qty = 50, Cost = 300m, Reorder = 30 },
                new { Id = "CON-001", Name = "Packaging Materials", Category = "Consumables", Qty = 1000, Cost = 5m, Reorder = 500 },
                new { Id = "CON-002", Name = "Labels", Category = "Consumables", Qty = 2000, Cost = 2m, Reorder = 1000 }
            };

            foreach (var product in initialProducts)
            {
                _gameState.Inventory[product.Id] = new InventoryItem
                {
                    ProductId = product.Id,
                    Quantity = product.Qty,
                    LastRestocked = _gameState.CurrentGameDate,
                    ReorderLevel = product.Reorder,
                    ReorderQuantity = product.Reorder * 2
                };
            }
        }

        private void RefreshData()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateMetrics();
                LoadInventoryItems();
                LoadReorderSuggestions();
                LoadCategoryDistribution();
            });
        }

        private void UpdateMetrics()
        {
            TotalItems = _gameState.Inventory.Sum(i => i.Value.Quantity);
            TotalInventoryValue = _gameState.Inventory.Sum(i => i.Value.Quantity * GetUnitCost(i.Key));
            AverageStockLevel = _gameState.Inventory.Count > 0
                ? (int)_gameState.Inventory.Average(i => i.Value.Quantity) : 0;
            TurnoverRate = 3.5 + (new Random().NextDouble() * 0.5 - 0.25);
            InventoryAccuracy = 0.98 + (new Random().NextDouble() * 0.02);
        }

        private void LoadInventoryItems()
        {
            var items = _gameState.Inventory
                .Select(i => CreateInventoryPageModel(i.Key, i.Value))
                .ToList();

            InventoryItems.Clear();
            foreach (var item in items)
                InventoryItems.Add(item);

            ShowingCount = InventoryItems.Count;
        }

        private InventoryItemPageModel CreateInventoryPageModel(string id, InventoryItem item)
        {
            var category = GetProductCategory(id);
            var unitCost = GetUnitCost(id);
            var maxStock = item.ReorderLevel * 3;

            return new InventoryItemPageModel
            {
                Id = id,
                SKU = id,
                ProductName = GetProductName(id),
                Description = GetProductDescription(id),
                Category = category,
                CategoryColor = GetCategoryColor(category),
                CurrentStock = item.Quantity,
                ReorderLevel = item.ReorderLevel,
                TotalValue = item.Quantity * unitCost,
                StockPercentage = (double)item.Quantity / maxStock,
                StockColor = GetStockColor(item.Quantity, item.ReorderLevel),
                Status = GetStockStatus(item.Quantity, item.ReorderLevel),
                StatusColor = GetStatusColor(item.Quantity, item.ReorderLevel)
            };
        }

        private void LoadReorderSuggestions()
        {
            ReorderSuggestions.Clear();
            var lowStockItems = _gameState.Inventory
                .Where(i => i.Value.Quantity <= i.Value.ReorderLevel)
                .Select(i => new ReorderSuggestion
                {
                    Id = i.Key,
                    ProductName = GetProductName(i.Key),
                    SKU = i.Key,
                    CurrentStock = i.Value.Quantity,
                    ReorderLevel = i.Value.ReorderLevel,
                    SuggestedQuantity = i.Value.ReorderQuantity
                })
                .ToList();

            foreach (var suggestion in lowStockItems)
                ReorderSuggestions.Add(suggestion);
        }

        private void LoadCategoryDistribution()
        {
            CategoryDistribution.Clear();
            var categories = _gameState.Inventory
                .GroupBy(i => GetProductCategory(i.Key))
                .Select(g => new CategoryDistribution
                {
                    Category = g.Key,
                    Value = g.Sum(i => i.Value.Quantity * GetUnitCost(i.Key)),
                    Color = GetCategoryColor(g.Key)
                })
                .OrderByDescending(c => c.Value)
                .ToList();

            foreach (var cat in categories)
                CategoryDistribution.Add(cat);
        }

        private void FilterInventory()
        {
            var filtered = _gameState.Inventory
                .Where(i =>
                {
                    bool matchesCategory = SelectedCategory == "All Categories" ||
                                          GetProductCategory(i.Key) == SelectedCategory;
                    bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) ||
                                        GetProductName(i.Key).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                        i.Key.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                    return matchesCategory && matchesSearch;
                })
                .Select(i => CreateInventoryPageModel(i.Key, i.Value))
                .ToList();

            InventoryItems.Clear();
            foreach (var item in filtered)
                InventoryItems.Add(item);

            ShowingCount = InventoryItems.Count;
        }

        [RelayCommand]
        private void AddItem()
        {
            var random = new Random();
            var newId = $"ITEM-{random.Next(1000, 9999)}";

            _gameState.Inventory[newId] = new InventoryItem
            {
                ProductId = newId,
                Quantity = random.Next(50, 200),
                LastRestocked = _gameState.CurrentGameDate,
                ReorderLevel = 100,
                ReorderQuantity = 200
            };
            RefreshData();
        }

        [RelayCommand]
        private void ViewItem(string itemId)
        {
            System.Diagnostics.Debug.WriteLine($"Viewing item: {itemId}");
        }

        [RelayCommand]
        private void ProcessReorder(string itemId)
        {
            if (!_gameState.Inventory.ContainsKey(itemId)) return;

            var item = _gameState.Inventory[itemId];
            var cost = item.ReorderQuantity * GetUnitCost(itemId);

            var transaction = new Transaction
            {
                Type = TransactionType.Expense,
                Amount = cost,
                Description = $"Reorder {GetProductName(itemId)}",
                Category = "Procurement",
                Date = _gameState.CurrentGameDate
            };

            if (_gameState.ProcessTransaction(transaction))
            {
                item.Quantity += item.ReorderQuantity;
                item.LastRestocked = _gameState.CurrentGameDate;
                RefreshData();
            }
        }

        [RelayCommand]
        private void StockReport() => System.Diagnostics.Debug.WriteLine("Generating stock report...");

        [RelayCommand]
        private void BatchUpdate() => System.Diagnostics.Debug.WriteLine("Opening batch update...");

        [RelayCommand]
        private void ExportInventory() => System.Diagnostics.Debug.WriteLine("Exporting inventory...");

        [RelayCommand]
        private void PhysicalCount() => System.Diagnostics.Debug.WriteLine("Starting physical count...");

        private void HandleGameEvent(GameEvent gameEvent)
        {
            switch (gameEvent.Type)
            {
                case EventType.ProductionCompleted:
                case EventType.OrderShipped:
                case EventType.MaterialShortage:
                    MainThread.BeginInvokeOnMainThread(() => RefreshData());
                    break;
            }
        }

        // Helper methods
        private string GetProductName(string id) => id switch
        {
            "RM-001" => "Steel Sheets",
            "RM-002" => "Aluminum Bars",
            "RM-003" => "Plastic Pellets",
            "COMP-001" => "Circuit Boards",
            "COMP-002" => "Motors",
            "COMP-003" => "Sensors",
            "FG-001" => "Product Alpha",
            "FG-002" => "Product Beta",
            "FG-003" => "Product Gamma",
            "CON-001" => "Packaging Materials",
            "CON-002" => "Labels",
            _ => $"Product {id}"
        };

        private string GetProductDescription(string id) => $"{GetProductCategory(id)} - {id}";

        private string GetProductCategory(string id)
        {
            if (id.StartsWith("RM-")) return "Raw Materials";
            if (id.StartsWith("COMP-")) return "Components";
            if (id.StartsWith("FG-")) return "Finished Goods";
            if (id.StartsWith("CON-")) return "Consumables";
            return "Other";
        }

        private decimal GetUnitCost(string id) => id switch
        {
            "RM-001" => 25m,
            "RM-002" => 30m,
            "RM-003" => 15m,
            "COMP-001" => 50m,
            "COMP-002" => 75m,
            "COMP-003" => 40m,
            "FG-001" => 200m,
            "FG-002" => 250m,
            "FG-003" => 300m,
            "CON-001" => 5m,
            "CON-002" => 2m,
            _ => 50m
        };

        private string GetCategoryColor(string category) => category switch
        {
            "Raw Materials" => "#4ECDC4",
            "Components" => "#FF6B6B",
            "Finished Goods" => "#06D6A0",
            "Consumables" => "#FFD166",
            _ => "#95A5A6"
        };

        private string GetStockColor(int current, int reorder)
        {
            if (current == 0) return "#EF4444";
            if (current <= reorder * 0.5) return "#F59E0B";
            if (current <= reorder) return "#FBBF24";
            return "#10B981";
        }

        private string GetStockStatus(int current, int reorder)
        {
            if (current == 0) return "Out";
            if (current <= reorder * 0.5) return "Critical";
            if (current <= reorder) return "Low";
            return "Good";
        }

        private string GetStatusColor(int current, int reorder)
        {
            if (current == 0) return "#EF4444";
            if (current <= reorder * 0.5) return "#F59E0B";
            if (current <= reorder) return "#FBBF24";
            return "#10B981";
        }

        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
            _gameState.OnGameEvent -= HandleGameEvent;
        }
    }
}