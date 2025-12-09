using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Warehouse
{
    public class ReorderSuggestion
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public int SuggestedQuantity { get; set; }
    }
}
