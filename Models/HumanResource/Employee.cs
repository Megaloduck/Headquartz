using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.HumanResource
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }

        public int SkillLevel { get; set; } // 1–100
        public double Salary { get; set; }

        public int Motivation { get; set; } // 1–100
        public bool IsTraining { get; set; }
    }

}
