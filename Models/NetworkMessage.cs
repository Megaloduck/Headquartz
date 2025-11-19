using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class NetworkMessage
    {
        public string Type { get; set; } = "";
        public string PlayerId { get; set; } = "";
        public string Data { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
