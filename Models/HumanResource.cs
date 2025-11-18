using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    public class HumanResource
    {
        public int Employees { get; set; } = 3;
        public double SalaryPerEmployee { get; set; } = 50;
        public double TotalSalaryCost => Employees * SalaryPerEmployee;
    }
}
