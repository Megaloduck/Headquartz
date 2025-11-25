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
            if (state.Warehouse.Products.Count == 0)
            {
                state.Warehouse.Products.Add(new Product("Basic Widget", 10)
                {
                    Stock = 50,
                    ProductionRate = 5
                });

                state.Warehouse.Products.Add(new Product("Premium Widget", 20)
                {
                    Stock = 25,
                    ProductionRate = 2
                });
            }

            if (state.Market.Products.Count == 0)
            {
                state.Market.Products.AddRange(state.Warehouse.Products);
            }
        }
    }
}
