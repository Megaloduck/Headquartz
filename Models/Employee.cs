using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class Employee
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Staff";
        public decimal Salary { get; set; } = 1000m;
        public double Efficiency { get; set; } = 1.0; // multiplier
    }
}
