using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Modules
{
    public static class InventoryModule
    {
        private static readonly Random _rand = new();

        public static void Update(GameState state)
        {
            foreach (var product in state.Inventory.Products)
            {
                // Produce items
                product.Stock += product.ProductionRate;

                // Sell items based on demand
                double demand = state.Market.CurrentDemand;
                int canSell = (int)Math.Min(product.Stock, demand);

                product.Stock -= canSell;

                // Money earned this tick
                double revenue = canSell * product.SellingPrice;

                state.Finance.RevenuePerTick += revenue;
            }
        }
    }
}
