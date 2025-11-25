using Headquartz.Models;
using Headquartz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Headquartz.PageModels
{
    public class DashboardPageModel : BasePageModel
    {
        private readonly GameState _state;
        private readonly ISimulationEngine _engine;

        public string TotalProducts => _state.Warehouse.Products.Count.ToString();
        public string LowStockText => $"{_state.Warehouse.Products.Count(p => p.Quantity < 10)} low stock";

        public string MonthlyRevenue => _state.Finance.Cash.ToString("C0");
        public string MonthlyRevenueChange => "+8.2% from last month";

        public string ActiveOrders => "2";
        public string OrdersThisMonth => "4";
        public string OrdersMonthChange => "+15%";

        public ObservableCollection<ActivityItem> RecentActivities { get; } = new();

        public DashboardPageModel(GameState state, ISimulationEngine engine)
        {
            _state = state;
            _engine = engine;

            // sample recent activities
            RecentActivities.Add(new ActivityItem("JS", "John Smith created sales order SO-2024-045", "21 minutes ago"));
            RecentActivities.Add(new ActivityItem("SJ", "Sarah Johnson updated stock levels for Laptop Computer", "34 minutes ago"));
            RecentActivities.Add(new ActivityItem("S", "System Low stock alert: USB-C Cable below reorder level", "1 hour ago"));

            _engine.OnTicked += s =>
            {
                // notify UI that computed properties changed
                OnPropertyChanged(nameof(TotalProducts));
                OnPropertyChanged(nameof(LowStockText));
                OnPropertyChanged(nameof(MonthlyRevenue));
            };
        }
    }

    public class ActivityItem
    {
        public string Initials { get; }
        public string Title { get; }
        public string Subtitle { get; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
        public string ActionIcon { get; set; }

        public ActivityItem(string initials, string title, string subtitle)
        {
            Initials = initials;
            Title = title;
            Subtitle = subtitle;
        }
    }
}
