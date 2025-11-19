using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "New Product";
        public double BasePrice { get; set; }
        public double SellingPrice { get; set; }
        public int Stock { get; set; }
        public int Quantity { get; set; } // For inventory tracking
        public int ProductionRate { get; set; }

        // Parameterless constructor for serialization
        public Product() { }

        public Product(string name, double basePrice)
        {
            Name = name;
            BasePrice = basePrice;
            SellingPrice = basePrice;
        }
    }
}
