using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class OptimizationSuggestion
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Impact { get; set; }
        public string ImpactColor { get; set; }
        public string Effort { get; set; }
    }
}
