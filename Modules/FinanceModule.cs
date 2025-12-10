using Headquartz.Models;

namespace Headquartz.Modules
{
    public static class FinanceModule
    {
        public static void Update(GameState state)
        {
            // Simple logic: Deduct daily fixed costs
            // only once per day? logic would need state tracking of "LastDayProcessed"

            // For now, let's just add a tiny amount of passive income every tick for visual feedback
            state.CashBalance += 10;
        }
    }
}
