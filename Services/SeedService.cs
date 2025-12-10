using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Services
{
    public static class SeedService
    {
        public static void Seed(GameState state)
        {
            if (state.Inventory.Count == 0)
            {
                // Seeding initial inventory

                var p1 = new Headquartz.Models.Warehouse.InventoryItem
                {
                    ProductId = "P-001",
                    Quantity = 50,
                    LastRestocked = state.CurrentGameDate
                };
                state.Inventory.Add(p1.ProductId, p1);


                var p2 = new Headquartz.Models.Warehouse.InventoryItem
                {
                    ProductId = "P-002",
                    Quantity = 25,
                    LastRestocked = state.CurrentGameDate
                };
                state.Inventory.Add(p2.ProductId, p2);
            }
        }
    }
}
