using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Simulation
{
    public sealed class TickContext
    {
        public int TickNumber { get; }
        public DateTime UtcTimestamp { get; }

        public TickContext(int tickNumber, DateTime utcTimestamp)
        {
            TickNumber = tickNumber;
            UtcTimestamp = utcTimestamp;
        }
    }
}
