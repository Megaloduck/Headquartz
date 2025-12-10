using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Domain
{
    public class CompanyInfo
    {
        public string Name { get; set; } = "Acme Co";
        public decimal Cash { get; set; } = 100000m;
        // add other company-wide KPIs here
    }
}
