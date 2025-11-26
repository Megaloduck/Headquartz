using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Models
{
    /// <summary>
    /// Represents a physical warehouse location
    /// </summary>
    public class WarehouseData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Main Warehouse";
        public string Location { get; set; } = "City Center";
        public double TotalCapacity { get; set; } = 10000; // in cubic meters
        public double UsedCapacity { get; set; } = 0;
        public double RentalCostPerMonth { get; set; } = 5000;
        public List<WarehouseZone> Zones { get; set; } = new();
        public WarehouseStatus Status { get; set; } = WarehouseStatus.Active;
    }

    /// <summary>
    /// Zones within a warehouse (e.g., Raw Materials, Finished Goods, Quarantine)
    /// </summary>
    public class WarehouseZone
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public ZoneType Type { get; set; }
        public double Capacity { get; set; }
        public double UsedSpace { get; set; }
        public List<string> ProductIds { get; set; } = new(); // Products stored here
    }

    public enum ZoneType
    {
        RawMaterials,
        WorkInProgress,
        FinishedGoods,
        Quarantine,
        Returns,
        Shipping,
        Receiving
    }

    public enum WarehouseStatus
    {
        Active,
        Maintenance,
        Full,
        Closed
    }

    /// <summary>
    /// Stock movement transaction
    /// </summary>
    public class StockTransaction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public TransactionType Type { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string FromLocation { get; set; } // Warehouse/Zone
        public string ToLocation { get; set; }
        public string PerformedBy { get; set; } // User/Role
        public string Reference { get; set; } // PO#, SO#, etc.
        public string Notes { get; set; }
        public double UnitCost { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
    }

    public enum TransactionType
    {
        Receipt,        // Goods received from supplier
        Issue,          // Goods issued to production/sales
        Transfer,       // Between locations
        Adjustment,     // Inventory correction
        Return,         // Customer/supplier return
        Damage,         // Write-off
        Production      // From/to production
    }

    public enum TransactionStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    /// <summary>
    /// Inventory item with warehouse details
    /// </summary>
    public class WarehouseInventoryItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderPoint { get; set; }
        public int MaximumStock { get; set; }
        public int ReorderQuantity { get; set; }
        public string PreferredLocation { get; set; }
        public DateTime LastCounted { get; set; }
        public int DaysInStock { get; set; }
        public double AverageDailyUsage { get; set; }
        public ItemCondition Condition { get; set; } = ItemCondition.Good;
        public List<StockLocation> Locations { get; set; } = new();
    }

    public class StockLocation
    {
        public string WarehouseId { get; set; }
        public string ZoneId { get; set; }
        public string Bin { get; set; } // Specific bin/shelf
        public int Quantity { get; set; }
    }

    public enum ItemCondition
    {
        Good,
        Damaged,
        Obsolete,
        Quarantine,
        PendingInspection
    }

    /// <summary>
    /// Stock alert/notification
    /// </summary>
    public class StockAlert
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public AlertType Type { get; set; }
        public AlertPriority Priority { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public bool IsResolved { get; set; } = false;
    }

    public enum AlertType
    {
        LowStock,
        Overstock,
        StockOut,
        ExpiringStock,
        DamagedStock,
        InventoryDiscrepancy,
        WarehouseFull
    }

    public enum AlertPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Picking/Fulfillment order
    /// </summary>
    public class PickingOrder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrderReference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public PickingStatus Status { get; set; } = PickingStatus.Pending;
        public string AssignedTo { get; set; }
        public List<PickingLine> Lines { get; set; } = new();
        public PickingPriority Priority { get; set; } = PickingPriority.Normal;
    }

    public class PickingLine
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityPicked { get; set; }
        public string PickLocation { get; set; }
    }

    public enum PickingStatus
    {
        Pending,
        InProgress,
        PartiallyPicked,
        Completed,
        Cancelled
    }

    public enum PickingPriority
    {
        Low,
        Normal,
        High,
        Express
    }

    /// <summary>
    /// Warehouse performance metrics
    /// </summary>
    public class WarehouseMetrics
    {
        public double InventoryAccuracy { get; set; } // %
        public double OrderFulfillmentRate { get; set; } // %
        public double StockTurnoverRatio { get; set; }
        public double AveragePickTime { get; set; } // minutes
        public double SpaceUtilization { get; set; } // %
        public int TotalTransactions { get; set; }
        public int ActiveAlerts { get; set; }
        public double CarryingCost { get; set; }
        public int ItemsAtReorderPoint { get; set; }
        public int StockOutItems { get; set; }
    }
}
