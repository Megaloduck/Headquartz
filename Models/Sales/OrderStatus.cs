using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models.Sales
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        InProduction,
        ReadyToShip,
        Shipped,
        Delivered,
        Cancelled
    }
}
