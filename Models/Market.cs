using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class Market
    {
        public double BaseDemand { get; set; } = 100;
        public double PriceSensitivity { get; set; } = 1.2;
        public double CurrentDemand { get; set; } = 100;
        public double TargetDemand { get; set; } = 100;
        public double Price { get; set; } = 50; // Add this
        public string TrendDescription { get; set; } = "➡ Stable Market"; // Add this
        public List<Product> Products { get; set; } = new();
        public decimal DemandFactor { get; set; } = 1.0m;
        public int Demand { get; set; } = 50;
    }
}
