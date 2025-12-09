using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;
using Headquartz.Models.Warehouse;

namespace Headquartz.PageModels.Warehouse
{
    public partial class ShipmentsPageModel : ObservableObject
    {
        private readonly GameState _gameState;
        private System.Timers.Timer _updateTimer;

        [ObservableProperty]
        private int todayShipments;

        [ObservableProperty]
        private int toShipToday;

        [ObservableProperty]
        private int inTransit;

        [ObservableProperty]
        private double onTimeRate;

        [ObservableProperty]
        private int avgDeliveryTime;

        [ObservableProperty]
        private string selectedCarrier;

        [ObservableProperty]
        private string selectedServiceType;

        [ObservableProperty]
        private DateTime shippingDate = DateTime.Now;

        [ObservableProperty]
        private string selectedPackaging;

        [ObservableProperty]
        private string shippingNotes;

        public ObservableCollection<ShippingScheduleItem> TodaysSchedule { get; } = new();
        public ObservableCollection<string> Carriers { get; } = new() { "FedEx", "UPS", "DHL", "USPS" };
        public ObservableCollection<string> ServiceTypes { get; } = new() { "Standard", "Express", "Overnight", "Economy" };
        public ObservableCollection<string> PackagingTypes { get; } = new() { "Box - Small", "Box - Medium", "Box - Large", "Pallet", "Envelope" };

        public ShipmentsPageModel()
        {
            _gameState = GameState.Instance;
            SelectedCarrier = "FedEx";
            SelectedServiceType = "Standard";
            SelectedPackaging = "Box - Medium";

            GenerateShippingSchedule();

            _updateTimer = new System.Timers.Timer(5000);
            _updateTimer.Elapsed += (s, e) => RefreshData();
            _updateTimer.Start();

            RefreshData();
        }

        private void GenerateShippingSchedule()
        {
            var random = new Random();
            var carriers = new[] { "FedEx", "UPS", "DHL" };
            var statuses = new[] { "Scheduled", "Ready", "Loaded" };
            var statusColors = new[] { "#F59E0B", "#10B981", "#3B82F6" };

            for (int i = 0; i < 6; i++)
            {
                var statusIndex = random.Next(3);
                TodaysSchedule.Add(new ShippingScheduleItem
                {
                    Id = Guid.NewGuid().ToString(),
                    TimeSlot = $"{8 + i * 2:D2}:00",
                    Carrier = carriers[random.Next(carriers.Length)],
                    OrdersCount = random.Next(5, 20),
                    Destination = new[] { "East Coast", "West Coast", "Midwest", "South" }[random.Next(4)],
                    PackageCount = random.Next(10, 50),
                    TotalWeight = random.Next(100, 500),
                    Status = statuses[statusIndex],
                    StatusColor = statusColors[statusIndex]
                });
            }
        }

        private void RefreshData()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var random = new Random();
                TodayShipments = TodaysSchedule.Count;
                ToShipToday = TodaysSchedule.Count(s => s.Status == "Scheduled");
                InTransit = random.Next(20, 50);
                OnTimeRate = 0.92 + (random.NextDouble() * 0.07);
                AvgDeliveryTime = random.Next(2, 5);
            });
        }

        [RelayCommand]
        private void PrepareShipment(string shipmentId)
        {
            var shipment = TodaysSchedule.FirstOrDefault(s => s.Id == shipmentId);
            if (shipment != null)
            {
                shipment.Status = "Ready";
                shipment.StatusColor = "#10B981";
            }
        }

        [RelayCommand]
        private void CreateShipment()
        {
            if (string.IsNullOrWhiteSpace(SelectedCarrier)) return;

            var newShipment = new ShippingScheduleItem
            {
                Id = Guid.NewGuid().ToString(),
                TimeSlot = DateTime.Now.ToString("HH:mm"),
                Carrier = SelectedCarrier,
                OrdersCount = 1,
                Destination = "Custom",
                PackageCount = 1,
                TotalWeight = 10,
                Status = "Scheduled",
                StatusColor = "#F59E0B"
            };

            TodaysSchedule.Add(newShipment);

            // Clear form
            ShippingNotes = "";
            RefreshData();
        }

        [RelayCommand]
        private void ShippingReport() => System.Diagnostics.Debug.WriteLine("Generating shipping report...");

        [RelayCommand]
        private void UpdateTracking() => System.Diagnostics.Debug.WriteLine("Updating tracking numbers...");

        [RelayCommand]
        private void PrintLabels() => System.Diagnostics.Debug.WriteLine("Printing shipping labels...");

        [RelayCommand]
        private void SchedulePickups() => System.Diagnostics.Debug.WriteLine("Scheduling carrier pickups...");

        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }
    }

   
}
