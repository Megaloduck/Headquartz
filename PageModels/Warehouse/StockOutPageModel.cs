using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models.Warehouse;
using Headquartz.Models.Domain;

namespace Headquartz.PageModels.Warehouse
{
    public partial class StockOutPageModel : ObservableObject
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _updateTimer;

        [ObservableProperty]
        private int todayShipments;

        [ObservableProperty]
        private int pendingOrders;

        [ObservableProperty]
        private int todaysTarget;

        [ObservableProperty]
        private int pickRate;

        [ObservableProperty]
        private double pickAccuracy;

        [ObservableProperty]
        private int toPickCount;

        [ObservableProperty]
        private int pickingCount;

        [ObservableProperty]
        private int toPackCount;

        [ObservableProperty]
        private int toShipCount;

        [ObservableProperty]
        private int packingQueue;

        [ObservableProperty]
        private int batchSize = 10;

        [ObservableProperty]
        private string selectedZone;

        [ObservableProperty]
        private string selectedPicker;

        [ObservableProperty]
        private int estimatedTime;

        public ObservableCollection<QuickPickItem> QuickPickItems { get; } = new();
        public ObservableCollection<PackingItem> PackingItems { get; } = new();
        public ObservableCollection<string> Zones { get; } = new() { "Zone A", "Zone B", "Zone C", "Zone D" };
        public ObservableCollection<string> Pickers { get; } = new() { "John D.", "Sarah M.", "Mike R.", "Lisa K." };

        public StockOutPageModel()
        {
            _gameState = GameState.Instance;
            SelectedZone = "Zone B";
            SelectedPicker = "John D.";

            GenerateQuickPickItems();
            GeneratePackingItems();

            _updateTimer = new System.Timers.Timer(5000);
            _updateTimer.Elapsed += (s, e) => RefreshData();
            _updateTimer.Start();

            RefreshData();
        }

        private void GenerateQuickPickItems()
        {
            var random = new Random();
            var priorities = new[] { "High", "Medium", "Low" };
            var priorityColors = new[] { "#EF4444", "#F59E0B", "#10B981" };

            for (int i = 0; i < 8; i++)
            {
                var priorityIndex = random.Next(3);
                QuickPickItems.Add(new QuickPickItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductName = GetProductName(_gameState.Inventory.Keys.ElementAt(random.Next(_gameState.Inventory.Count))),
                    SKU = _gameState.Inventory.Keys.ElementAt(random.Next(_gameState.Inventory.Count)),
                    Location = $"{(char)('A' + random.Next(4))}-{random.Next(1, 10):D2}-{random.Next(1, 20):D2}",
                    OrderNumber = $"ORD-{random.Next(1000, 9999)}",
                    Quantity = random.Next(1, 50),
                    Priority = priorities[priorityIndex],
                    PriorityColor = priorityColors[priorityIndex]
                });
            }
        }

        private void GeneratePackingItems()
        {
            var random = new Random();
            for (int i = 0; i < 5; i++)
            {
                PackingItems.Add(new PackingItem
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderNumber = $"ORD-{random.Next(1000, 9999)}",
                    ItemCount = random.Next(1, 10),
                    Picker = Pickers[random.Next(Pickers.Count)],
                    PickTime = _gameState.CurrentGameDate.AddHours(-random.Next(1, 5))
                });
            }
        }

        private void RefreshData()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var random = new Random();
                TodayShipments = random.Next(15, 35);
                PendingOrders = _gameState.ActiveSalesOrders.Count;
                TodaysTarget = 50;
                PickRate = 25 + random.Next(-5, 10);
                PickAccuracy = 0.96 + (random.NextDouble() * 0.04);

                ToPickCount = QuickPickItems.Count;
                PickingCount = random.Next(5, 15);
                ToPackCount = PackingItems.Count;
                ToShipCount = random.Next(10, 20);
                PackingQueue = PackingItems.Count;

                EstimatedTime = BatchSize * 3;
            });
        }

        [RelayCommand]
        private void PickItem(string itemId)
        {
            var item = QuickPickItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                QuickPickItems.Remove(item);

                // Add to packing queue
                PackingItems.Add(new PackingItem
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderNumber = item.OrderNumber,
                    ItemCount = 1,
                    Picker = SelectedPicker,
                    PickTime = _gameState.CurrentGameDate
                });

                ToPickCount = QuickPickItems.Count;
                PackingQueue = PackingItems.Count;
            }
        }

        [RelayCommand]
        private void CreatePickBatch()
        {
            System.Diagnostics.Debug.WriteLine($"Creating batch of {BatchSize} orders for {SelectedPicker} in {SelectedZone}");
        }

        [RelayCommand]
        private void PackOrder(string orderId)
        {
            var item = PackingItems.FirstOrDefault(p => p.Id == orderId);
            if (item != null)
            {
                PackingItems.Remove(item);
                ToShipCount++;
                PackingQueue = PackingItems.Count;
            }
        }

        [RelayCommand]
        private void FulfillmentReport() => System.Diagnostics.Debug.WriteLine("Generating fulfillment report...");

        [RelayCommand]
        private void OptimizeRoute() => System.Diagnostics.Debug.WriteLine("Optimizing pick route...");

        [RelayCommand]
        private void ScanToPick() => System.Diagnostics.Debug.WriteLine("Opening scan to pick...");

        [RelayCommand]
        private void CompleteBatch()
        {
            QuickPickItems.Clear();
            ToPickCount = 0;
            System.Diagnostics.Debug.WriteLine("Batch completed!");
        }

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

        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }
    }  

    
}
