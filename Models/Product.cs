using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "New Product";
        public int Inventory { get; set; } = 0;
        public decimal Price { get; set; } = 10m;
        public decimal Cost { get; set; } = 5m;
    }
}
