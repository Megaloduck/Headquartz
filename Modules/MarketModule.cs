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
        // simple random-based demand fluctuation
        private static Random _rnd = new();


        public static void Update(GameState state)
        {
            // jitter demand factor +/- 5%
            var change = (decimal)(_rnd.NextDouble() * 0.1 - 0.05);
            state.Market.DemandFactor = Math.Max(0.1m, state.Market.DemandFactor * (1 + change));


            // simulate simple sales: for each product, sell floor(DemandFactor * some base)
            foreach (var p in state.Company.Products)
            {
                var baseDemand = 1 + (int)(state.Market.DemandFactor * 2);
                var sold = Math.Min(p.Inventory, baseDemand);
                p.Inventory -= sold;
                var revenue = sold * p.Price;
                state.Company.Cash += revenue;
            }


            // pay salaries every 60 ticks (if TickRate=1s, equals every minute)
            // for brevity, omitted — implement later in HRModule.
        }
    }
}
