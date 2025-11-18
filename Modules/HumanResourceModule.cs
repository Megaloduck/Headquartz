using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Modules
{
    public static class HumanResourceModule
    {
        public static void Update(GameState state)
        {
            // Wage expense
            double salaryCost = state.HumanResource.Employees * state.HumanResource.SalaryPerEmployee;

            state.Finance.ExpensePerTick += salaryCost;
        }
    }
}
