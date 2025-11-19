using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class GameState
    {
        public DateTime SimTime { get; set; } = DateTime.UtcNow;
        public PlayerRole CurrentRole { get; set; }


        // Core Systems
        public Company Company { get; set; } = new();
        public Market Market { get; set; } = new();
        public HumanResource HumanResource { get; set; } = new();
        public Finance Finance { get; set; } = new();
        public Inventory Inventory { get; set; } = new();

        public class InventoryState
        {
            public List<Product> Products { get; set; } = new()
            {
                new Product { Name = "Water Bottles", Quantity = 50 },
                new Product { Name = "Snacks", Quantity = 40 },
                new Product { Name = "Electronics", Quantity = 20 },
            };
        }

        public class Product
        {
            public string Name { get; set; } = "";
            public int Quantity { get; set; }
        }
    }


}
