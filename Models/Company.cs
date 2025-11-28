using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class Company
    {
        public string Name { get; set; } = "My Company";
        public decimal Cash { get; set; } = 100000m;
        public int Level { get; set; } = 1;
        public List<Product> Products { get; set; } = new();
    }
}
