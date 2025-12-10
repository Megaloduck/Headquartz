using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models.Warehouse;
using Headquartz.Models.Sales;
using Headquartz.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Headquartz.Models.Domain;

namespace Headquartz.PageModels.Warehouse
{
    public partial class WarehouseDashboardPageModel : ObservableObject
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _updateTimer;

        [ObservableProperty]
        private int totalSKUs;

        [ObservableProperty]
        private decimal totalInventoryValue;

        [ObservableProperty]
        private double spaceUtilization;

        [ObservableProperty]
        private string utilizationColor;

        [ObservableProperty]
        private int todaysMovements;

        [ObservableProperty]
        private int pendingOrders;

        [ObservableProperty]
        private int receivingCount;

        [ObservableProperty]
        private int storageCount;

        [ObservableProperty]
        private int shippingCount;

        [ObservableProperty]
        private int lowStockCount;

        [ObservableProperty]
        private double zoneAUsage;

        [ObservableProperty]
        private string zoneAColor;

        [ObservableProperty]
        private double zoneBUsage;

        [ObservableProperty]
        private string zoneBColor;

        [ObservableProperty]
        private double zoneCUsage;

        [ObservableProperty]
        private string zoneCColor;

        [ObservableProperty]
        private double zoneDUsage;

        [ObservableProperty]
        private string zoneDColor;

        public ObservableCollection<LowStockItem> LowStockItems { get; } = new();
        public ObservableCollection<WarehouseMovement> RecentMovements { get; } = new();

        public WarehouseDashboardPageModel()
        {
            _gameState = GameState.Instance;

            // Subscribe to game events
            _gameState.OnGameEvent += HandleGameEvent;

            // Initialize data
            InitializeWarehouseData();

            // Start update timer (refresh every 5 seconds)
            _updateTimer = new System.Timers.Timer(5000);
            _updateTimer.Elapsed += (s, e) => RefreshData();
            _updateTimer.Start();
        }

        private void InitializeWarehouseData()
        {
            // Initialize inventory if empty
            if (_gameState.Inventory.Count == 0)
            {
                InitializeStartingInventory();
            }

            RefreshData();
        }

        private void InitializeStartingInventory()
        {
            var products = new[]
            {
                new { Id = "PROD-001", Name = "Widget A", Qty = 500, Icon = "🔧" },
                new { Id = "PROD-002", Name = "Widget B", Qty = 350, Icon = "⚙️" },
                new { Id = "PROD-003", Name = "Gadget X", Qty = 200, Icon = "🔩" },
                new { Id = "PROD-004", Name = "Component Y", Qty = 150, Icon = "🔨" },
                new { Id = "PROD-005", Name = "Assembly Z", Qty = 80, Icon = "🛠️" }
            };

            foreach (var product in products)
            {
                _gameState.Inventory[product.Id] = new InventoryItem
                {
                    ProductId = product.Id,
                    Quantity = product.Qty,
                    LastRestocked = _gameState.CurrentGameDate,
                    ReorderLevel = 100,
                    ReorderQuantity = 200
                };
            }
        }

        private void RefreshData()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateInventoryMetrics();
                UpdateWarehouseZones();
                UpdateLowStockAlerts();
                UpdateRecentMovements();
                UpdateOrderCounts();
            });
        }

        private void UpdateInventoryMetrics()
        {
            TotalSKUs = _gameState.Inventory.Count;
            TotalInventoryValue = _gameState.Inventory.Sum(i => i.Value.Quantity * 50m) / 1000m; // Simplified value

            var totalCapacity = 10000; // Total warehouse capacity
            var currentStock = _gameState.Inventory.Sum(i => i.Value.Quantity);
            SpaceUtilization = (double)currentStock / totalCapacity;

            // Color coding for utilization
            UtilizationColor = SpaceUtilization switch
            {
                >= 0.9 => "#EF4444", // Red - Critical
                >= 0.75 => "#F59E0B", // Orange - Warning
                >= 0.5 => "#10B981", // Green - Good
                _ => "#3B82F6" // Blue - Low
            };

            TodaysMovements = RecentMovements.Count(m => m.MovementTime.Date == _gameState.CurrentGameDate.Date);
        }

        private void UpdateWarehouseZones()
        {
            var random = new Random();

            // Zone A: Bulk Storage
            ZoneAUsage = 0.65 + (random.NextDouble() * 0.1 - 0.05);
            ZoneAColor = ZoneAUsage >= 0.8 ? "#F59E0B" : "#10B981";

            // Zone B: Picking Zone
            ZoneBUsage = 0.75 + (random.NextDouble() * 0.1 - 0.05);
            ZoneBColor = ZoneBUsage >= 0.8 ? "#F59E0B" : "#10B981";

            // Zone C: Cold Storage
            ZoneCUsage = 0.45 + (random.NextDouble() * 0.1 - 0.05);
            ZoneCColor = ZoneCUsage >= 0.8 ? "#F59E0B" : "#10B981";

            // Zone D: Hazardous
            ZoneDUsage = 0.30 + (random.NextDouble() * 0.1 - 0.05);
            ZoneDColor = ZoneDUsage >= 0.8 ? "#F59E0B" : "#10B981";
        }

        private void UpdateLowStockAlerts()
        {
            LowStockItems.Clear();

            var lowStockProducts = _gameState.Inventory
                .Where(i => i.Value.Quantity < i.Value.ReorderLevel)
                .Select(i => new LowStockItem
                {
                    Id = i.Key,
                    ProductName = GetProductName(i.Key),
                    SKU = i.Key,
                    Icon = GetProductIcon(i.Key),
                    CurrentStock = i.Value.Quantity,
                    ReorderLevel = i.Value.ReorderLevel
                })
                .ToList();

            foreach (var item in lowStockProducts)
            {
                LowStockItems.Add(item);
            }

            LowStockCount = LowStockItems.Count;
        }

        private void UpdateRecentMovements()
        {
            // Keep only recent movements (last 20)
            while (RecentMovements.Count > 20)
            {
                RecentMovements.RemoveAt(0);
            }
        }

        private void UpdateOrderCounts()
        {
            ReceivingCount = _gameState.ActiveSalesOrders.Count(o => o.Status == OrderStatus.Pending);
            StorageCount = _gameState.Inventory.Sum(i => i.Value.Quantity);
            ShippingCount = _gameState.ActiveSalesOrders.Count(o => o.Status == OrderStatus.ReadyToShip);
            PendingOrders = _gameState.ActiveSalesOrders.Count(o =>
                o.Status == OrderStatus.Pending ||
                o.Status == OrderStatus.Confirmed);
        }

        private void HandleGameEvent(GameEvent gameEvent)
        {
            switch (gameEvent.Type)
            {
                case EventType.NewSalesOrder:
                    AddMovement("Inbound", "New order received", "#4ECDC4", "⬇️");
                    break;

                case EventType.OrderShipped:
                    AddMovement("Outbound", "Order shipped", "#06D6A0", "⬆️");
                    break;

                case EventType.ProductionCompleted:
                    AddMovement("Receiving", "Production completed", "#FFD166", "📦");
                    break;

                case EventType.MaterialShortage:
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Could show notification or update UI
                        RefreshData();
                    });
                    break;
            }
        }

        private void AddMovement(string type, string productName, string color, string icon)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var random = new Random();
                var movement = new WarehouseMovement
                {
                    Id = Guid.NewGuid().ToString(),
                    MovementType = type,
                    ProductName = productName,
                    Quantity = random.Next(10, 100),
                    MovementTime = _gameState.CurrentGameDate,
                    Status = "Completed",
                    StatusColor = "#10B981",
                    TypeColor = color,
                    Icon = icon,
                    QuantityColor = type == "Outbound" ? "#EF4444" : "#10B981"
                };

                RecentMovements.Insert(0, movement);

                // Keep only last 20 movements
                while (RecentMovements.Count > 20)
                {
                    RecentMovements.RemoveAt(RecentMovements.Count - 1);
                }

                TodaysMovements = RecentMovements.Count(m => m.MovementTime.Date == _gameState.CurrentGameDate.Date);
            });
        }

        [RelayCommand]
        private void Reorder(string productId)
        {
            var inventoryItem = _gameState.Inventory[productId];

            // Create purchase order
            var purchaseOrder = new PurchaseOrder
            {
                Id = Guid.NewGuid().ToString(),
                SupplierId = "SUPP-001",
                OrderDate = _gameState.CurrentGameDate,
                ExpectedDeliveryDate = _gameState.CurrentGameDate.AddDays(7),
                Items = new List<PurchaseOrderLine>
                {
                    new PurchaseOrderLine
                    {
                        MaterialId = productId,
                        Quantity = inventoryItem.ReorderQuantity,
                        UnitCost = 25m
                    }
                },
                TotalCost = inventoryItem.ReorderQuantity * 25m
            };

            // Process transaction
            var transaction = new Transaction
            {
                Type = TransactionType.Expense,
                Amount = purchaseOrder.TotalCost,
                Description = $"Purchase Order #{purchaseOrder.Id}",
                Category = "Procurement",
                Date = _gameState.CurrentGameDate
            };

            if (_gameState.ProcessTransaction(transaction))
            {
                AddMovement("Purchase Order", GetProductName(productId), "#4ECDC4", "🛒");

                // Schedule delivery
                ScheduleDelivery(productId, inventoryItem.ReorderQuantity, 7);
            }
        }

        private void ScheduleDelivery(string productId, int quantity, int daysUntilDelivery)
        {
            // In a full implementation, this would schedule an actual delivery
            // For now, we'll just add it immediately with a notification
            Task.Delay(TimeSpan.FromSeconds(daysUntilDelivery * 2)).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_gameState.Inventory.ContainsKey(productId))
                    {
                        _gameState.Inventory[productId].Quantity += quantity;
                        _gameState.Inventory[productId].LastRestocked = _gameState.CurrentGameDate;
                    }

                    AddMovement("Receiving", $"Delivered: {GetProductName(productId)}", "#10B981", "✅");
                    RefreshData();
                });
            });
        }

        [RelayCommand]
        private void ReceiveGoods()
        {
            // Navigate to receiving page or show receiving dialog
            // For demo, simulate receiving
            var random = new Random();
            var productId = $"PROD-{random.Next(1, 6):D3}";

            AddMovement("Receiving", $"Manual receipt: {GetProductName(productId)}", "#4ECDC4", "📥");
        }

        [RelayCommand]
        private void InventoryCheck()
        {
            // Trigger inventory audit
            AddMovement("Audit", "Inventory check initiated", "#FFD166", "📋");
            RefreshData();
        }

        [RelayCommand]
        private void PrepareShipment()
        {
            // Check for orders ready to ship
            var readyOrders = _gameState.ActiveSalesOrders
                .Where(o => o.Status == OrderStatus.ReadyToShip)
                .ToList();

            if (readyOrders.Any())
            {
                var order = readyOrders.First();
                order.Status = OrderStatus.Shipped;

                AddMovement("Shipping", $"Order #{order.Id} prepared", "#06D6A0", "📤");
            }
        }

        private string GetProductName(string productId)
        {
            return productId switch
            {
                "PROD-001" => "Widget A",
                "PROD-002" => "Widget B",
                "PROD-003" => "Gadget X",
                "PROD-004" => "Component Y",
                "PROD-005" => "Assembly Z",
                _ => $"Product {productId}"
            };
        }

        private string GetProductIcon(string productId)
        {
            return productId switch
            {
                "PROD-001" => "🔧",
                "PROD-002" => "⚙️",
                "PROD-003" => "🔩",
                "PROD-004" => "🔨",
                "PROD-005" => "🛠️",
                _ => "📦"
            };
        }

        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
            _gameState.OnGameEvent -= HandleGameEvent;
        }
    }
}
