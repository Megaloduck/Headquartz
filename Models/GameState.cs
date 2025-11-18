using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class GameState
    {
        public DateTime SimTime { get; set; } = DateTime.UtcNow;

        // Core Systems
        public Company Company { get; set; } = new();
        public Market Market { get; set; } = new();
        public Inventory Inventory { get; set; } = new();
        public HumanResource HumanResource { get; set; } = new();
        public Finance Finance { get; set; } = new();
    }
}
