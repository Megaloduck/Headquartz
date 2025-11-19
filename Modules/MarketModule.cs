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
        static Random _rnd = new();

        public static void Update(GameState state)
        {
            state.Market.Demand = Math.Clamp(
                state.Market.Demand + _rnd.Next(-3, 6),
                0,
                100);

            state.Market.Price = Math.Clamp(
                state.Market.Price + (_rnd.NextSingle() - 0.5f),
                1f,
                100f);

            state.Market.TrendDescription = state.Market.Price switch
            {
                < 20 => "🔥 Prices Falling",
                < 50 => "➡ Stable Market",
                _ => "📈 Prices Rising"
            };
        }

        public static string GenerateForecast(GameState state)
        {
            return $"In 3 days, demand is expected to be ≈ {state.Market.Demand + _rnd.Next(-10, 10)}.";
        }
    }

}
