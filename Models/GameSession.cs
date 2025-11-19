using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class GameSession
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public string HostName { get; set; } = "Unknown Host";
        public string HostIp { get; set; } = "";
        public int Players { get; set; }
        public int MaxPlayers { get; set; } = 8;

        public bool IsPasswordProtected { get; set; } = false;

        public override string ToString()
        {
            return $"{HostName} ({Players}/{MaxPlayers})";
        }
    }
}