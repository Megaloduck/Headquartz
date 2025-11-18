using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Modules
{
    public static class MarketModule
    {
        private static readonly Random _rand = new();

        public static void Update(GameState state)
        {
            foreach (var product in state.Market.Products)
            {
                // Price elasticity
                double elasticity = Math.Max(0.2,
                    1 - ((product.SellingPrice - product.BasePrice) / product.BasePrice));

                // Random market fluctuation (±10%)
                double randomFactor = 1 + (_rand.NextDouble() * 0.2 - 0.1);

                // Demand score
                state.Market.CurrentDemand =
                    state.Market.BaseDemand * elasticity * randomFactor;

                // Must not go negative
                state.Market.CurrentDemand = Math.Max(0, state.Market.CurrentDemand);
            }
        }
    }
}
