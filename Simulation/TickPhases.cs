using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Simulation
{
    // Define phase ordering for a tick. Modules may implement only the phases they need.
    public enum TickPhase
    {
        CollectActions = 0,
        Production = 10,
        Inventory = 20,
        Market = 30,
        Finance = 40,
        Events = 50,
        Broadcast = 100
    }
}
