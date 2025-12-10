using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models.Domain;
using Headquartz.Models.Warehouse;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Headquartz.PageModels.Warehouse
{
    public partial class StockInPageModel : ObservableObject
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _updateTimer;

        [ObservableProperty]
        private int todayReceipts;

        [ObservableProperty]
        private int pendingReceipts;

        [ObservableProperty]
        private int avgProcessingTime;

        [ObservableProperty]
        private int itemsReceived;

        [ObservableProperty]
        private double qualityPassRate;

        [ObservableProperty]
        private int avgDelay;

        [ObservableProperty]
        private int qualityCheckQueue;

        [ObservableProperty]
        private bool hasPendingReceipts;

        // Form fields
        [ObservableProperty]
        private string selectedSupplier;

        [ObservableProperty]
        private string poNumber;

        [ObservableProperty]
        private string selectedProduct;

        [ObservableProperty]
        private string quantity;

        [ObservableProperty]
        private string batchNumber;

        [ObservableProperty]
        private DateTime expiryDate = DateTime.Now.AddMonths(6);

        [ObservableProperty]
        private string receiptNotes;

        public ObservableCollection<string> Suppliers { get; } = new();
        public ObservableCollection<string> Products { get; } = new();
        public ObservableCollection<PendingReceiptItem> PendingReceiptsList { get; } = new();
        public ObservableCollection<QualityCheckItem> QualityChecks { get; } = new();

        public StockInPageModel()
        {
            _gameState = GameState.Instance;
            InitializeData();

            _updateTimer = new System.Timers.Timer(5000);
            _updateTimer.Elapsed += (s, e) => RefreshData();
            _updateTimer.Start();
        }

        private void InitializeData()
        {
            // Suppliers
            Suppliers.Add("Supplier A - Steel Corp");
            Suppliers.Add("Supplier B - Tech Components");
            Suppliers.Add("Supplier C - Plastics Inc");
            Suppliers.Add("Supplier D - Electronics Ltd");

            // Products (from inventory)
            foreach (var item in _gameState.Inventory.Keys)
            {
                Products.Add($"{item} - {GetProductName(item)}");
            }

            // Generate pending receipts
            GeneratePendingReceipts();
            GenerateQualityChecks();
            RefreshData();
        }

        private void GeneratePendingReceipts()
        {
            var random = new Random();
            for (int i = 0; i < 5; i++)
            {
                PendingReceiptsList.Add(new PendingReceiptItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ReceiptNumber = $"RCV-{1000 + i}",
                    SupplierName = Suppliers[random.Next(Suppliers.Count)],
                    ProductName = GetProductName(_gameState.Inventory.Keys.ElementAt(random.Next(_gameState.Inventory.Count))),
                    PONumber = $"PO-{random.Next(1000, 9999)}",
                    Quantity = random.Next(50, 500),
                    ExpectedDate = _gameState.CurrentGameDate.AddDays(random.Next(-2, 3)),
                    Status = random.Next(3) switch
                    {
                        0 => "Pending",
                        1 => "In Transit",
                        _ => "Arrived"
                    },
                    StatusColor = random.Next(3) switch
                    {
                        0 => "#F59E0B",
                        1 => "#3B82F6",
                        _ => "#10B981"
                    }
                });
            }
        }

        private void GenerateQualityChecks()
        {
            var random = new Random();
            for (int i = 0; i < 3; i++)
            {
                QualityChecks.Add(new QualityCheckItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductName = GetProductName(_gameState.Inventory.Keys.ElementAt(random.Next(_gameState.Inventory.Count))),
                    BatchNumber = $"BATCH-{random.Next(1000, 9999)}",
                    ReceivedDate = _gameState.CurrentGameDate,
                    Inspector = $"Inspector {(char)('A' + random.Next(0, 5))}"
                });
            }
        }

        private void RefreshData()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TodayReceipts = PendingReceiptsList.Count(r => r.ExpectedDate.Date == _gameState.CurrentGameDate.Date);
                PendingReceipts = PendingReceiptsList.Count(r => r.Status == "Pending");
                AvgProcessingTime = 15 + new Random().Next(-5, 10);
                ItemsReceived = PendingReceiptsList.Sum(r => r.Status == "Arrived" ? r.Quantity : 0);
                QualityPassRate = 0.95 + (new Random().NextDouble() * 0.05);
                AvgDelay = new Random().Next(0, 8);
                QualityCheckQueue = QualityChecks.Count;
                HasPendingReceipts = PendingReceipts > 0;
            });
        }

        [RelayCommand]
        private void NewReceipt()
        {
            //PONumber = "";
            Quantity = "";
            BatchNumber = "";
            ReceiptNotes = "";
        }

        [RelayCommand]
        private void ScanBarcode()
        {
            System.Diagnostics.Debug.WriteLine("Opening barcode scanner...");
        }

        [RelayCommand]
        private void ReceiveGoods()
        {
            if (string.IsNullOrWhiteSpace(SelectedProduct) || string.IsNullOrWhiteSpace(Quantity))
                return;

            var productId = SelectedProduct.Split('-')[0].Trim();
            if (!int.TryParse(Quantity, out int qty)) return;

            if (_gameState.Inventory.ContainsKey(productId))
            {
                _gameState.Inventory[productId].Quantity += qty;
                _gameState.Inventory[productId].LastRestocked = _gameState.CurrentGameDate;

                // Clear form
                //PONumber = "";
                Quantity = "";
                BatchNumber = "";
                ReceiptNotes = "";

                RefreshData();
            }
        }

        [RelayCommand]
        private void ProcessReceipt(string receiptId)
        {
            var receipt = PendingReceiptsList.FirstOrDefault(r => r.Id == receiptId);
            if (receipt != null)
            {
                receipt.Status = "Completed";
                receipt.StatusColor = "#10B981";
                RefreshData();
            }
        }

        [RelayCommand]
        private void PerformQualityCheck(string checkId)
        {
            var check = QualityChecks.FirstOrDefault(c => c.Id == checkId);
            if (check != null)
            {
                QualityChecks.Remove(check);
                QualityCheckQueue = QualityChecks.Count;
            }
        }

        [RelayCommand]
        private void ReceivingReport() => System.Diagnostics.Debug.WriteLine("Generating receiving report...");

        [RelayCommand]
        private void BatchProcess() => System.Diagnostics.Debug.WriteLine("Batch processing receipts...");

        [RelayCommand]
        private void ExportReceipts() => System.Diagnostics.Debug.WriteLine("Exporting receipts...");

        [RelayCommand]
        private void CompleteAll()
        {
            foreach (var receipt in PendingReceiptsList.Where(r => r.Status != "Completed"))
            {
                receipt.Status = "Completed";
                receipt.StatusColor = "#10B981";
            }
            RefreshData();
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