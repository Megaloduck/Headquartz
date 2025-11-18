using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Modules
{
    public static class FinanceModule
    {
        public static void Update(GameState state)
        {
            double revenue = state.Finance.RevenuePerTick;
            double expense = state.Finance.ExpensePerTick;

            double net = revenue - expense;

            state.Finance.Cash += net;

            // Reset for next tick
            state.Finance.RevenuePerTick = 0;
            state.Finance.ExpensePerTick = 0;
        }
    }
}
