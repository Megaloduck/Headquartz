using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class MultiplayerSession
    {
        public string SessionId { get; set; }
        public string HostPlayerId { get; set; }
        public Dictionary<string, PlayerInfo> Players { get; set; }
        public GameState GameState { get; set; }
        public DateTime SessionStartTime { get; set; }
        public TimeSpan GameSpeed { get; set; }
        public bool IsPaused { get; set; }
    }
}
