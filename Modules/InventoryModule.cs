using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Modules
{
    public static class InventoryModule
    {
        private static readonly Random _rnd = new();

        public static void Update(GameState state)
        {
            foreach (var p in state.Inventory.Products)
            {
                // products get consumed randomly
                int used = _rnd.Next(0, 3); // 0–2
                p.Quantity = Math.Max(0, p.Quantity - used);
            }
        }
    }
}
