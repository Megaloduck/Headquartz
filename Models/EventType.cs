using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public enum EventType
    {
        NewSalesOrder,
        OrderShipped,
        ProductionCompleted,
        MaterialShortage,
        LowCashFlow,
        InsufficientFunds,
        PayrollProcessed,
        MarketChange,
        MachineBreakdown,
        MajorOrder,
        SupplierDelay,
        QualityIssue,
        MarketOpportunity,
        BudgetReview,
        FinancialReport
    }
}
