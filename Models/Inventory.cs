using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class Warehouse
    {
        public List<Product> Products { get; set; } = new();

        public int TotalItems => Products.Sum(p => p.Stock);
    }
}
