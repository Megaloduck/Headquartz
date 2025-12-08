using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class PlayerInfo
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public Dictionary<string, PlayerRole> AssignedRole { get; set; } // CEO, Finance, HR, etc.    
        public bool IsConnected { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
