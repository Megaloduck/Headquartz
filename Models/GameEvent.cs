using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class GameEvent
    {
        public EventType Type { get; set; }
        public string Message { get; set; }
        public EventSeverity Severity { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
