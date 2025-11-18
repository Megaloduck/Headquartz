using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class Finance
    {
        public double Cash { get; set; } = 10000;
        public double RevenuePerTick { get; set; }
        public double ExpensePerTick { get; set; }
        public double ProfitPerTick => RevenuePerTick - ExpensePerTick;
    }
}
