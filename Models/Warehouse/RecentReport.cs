using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class RecentReport
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string ReportColor { get; set; }
        public DateTime GeneratedDate { get; set; }
    }
}
