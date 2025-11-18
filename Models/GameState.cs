using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class GameState
    {
        public Company Company { get; set; } = new Company();
        public Market Market { get; set; } = new Market();
        public DateTime SimTime { get; set; } = DateTime.UtcNow;
    }


    public class Market
    {
        public decimal DemandFactor { get; set; } = 1.0m; // affects sales
    }
}
