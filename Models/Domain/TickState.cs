using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Global
{
    public class TickState
    {
        public int Current { get; set; }
        public int TickIntervalMs { get; set; } = 1000;
    }
}
