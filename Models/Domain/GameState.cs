using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using Headquartz.Models.HumanResource;
using Headquartz.Models.Production;
using Headquartz.Models.Sales;
using Headquartz.Models.Warehouse;
using Headquartz;


namespace Headquartz.Models.Domain
{
    /// <summary>
    /// Central game state shared across all players in LAN multiplayer
    /// This should be synchronized via network
    /// </summary>
    public class GameState : INotifyPropertyChanged
    {
        private static GameState _instance;
        public static GameState Instance => _instance ??= new GameState();

        // Time Management
        private DateTime _currentGameDate = new DateTime(2025, 1, 1);
        private int _gameDay = 1;
        private int _gameWeek = 1;
        private int _gameMonth = 1;
        private int _gameYear = 1;
        private TimeSpan _gameSpeed = TimeSpan.FromSeconds(10); // 1 game day = 10 real seconds

        // Financial Metrics
        private decimal _cashBalance = 1000000m; // Starting capital
        private decimal _monthlyRevenue = 0m;
        private decimal _monthlyExpenses = 0m;
        private decimal _totalAssets = 1000000m;
        private decimal _totalLiabilities = 0m;

        // Company Performance
        private decimal _companyValue = 1000000m;
        private int _customerSatisfaction = 80;
        private int _employeeSatisfaction = 75;
        private int _marketShare = 5;

        // Inventory
        private Dictionary<string, InventoryItem> _inventory = new();
        private Dictionary<string, RawMaterial> _rawMaterials = new();

        // Orders & Production
        private List<SalesOrder> _activeSalesOrders = new();
        private List<WorkOrder> _activeWorkOrders = new();
        private Queue<PurchaseOrder> _pendingPurchaseOrders = new();

        // Employees
        private List<Employee> _employees = new();
        private int _totalEmployees = 50; // Starting workforce
        private decimal _monthlyPayroll = 250000m;

        // Market Conditions
        private decimal _marketDemandMultiplier = 1.0m;
        private decimal _competitorPriceIndex = 1.0m;
        private Dictionary<string, decimal> _productDemand = new();

        // Events & Notifications
        public event Action<GameEvent> OnGameEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        // Properties with notification
        public DateTime CurrentGameDate
        {
            get => _currentGameDate;
            set { _currentGameDate = value; OnPropertyChanged(); }
        }

        public int GameDay
        {
            get => _gameDay;
            set { _gameDay = value; OnPropertyChanged(); }
        }

        public int GameWeek
        {
            get => _gameWeek;
            set { _gameWeek = value; OnPropertyChanged(); }
        }

        public int GameMonth
        {
            get => _gameMonth;
            set { _gameMonth = value; OnPropertyChanged(); }
        }

        public int GameYear
        {
            get => _gameYear;
            set { _gameYear = value; OnPropertyChanged(); }
        }

        public decimal CashBalance
        {
            get => _cashBalance;
            set
            {
                _cashBalance = value;
                OnPropertyChanged();
                CheckCashFlowAlerts();
            }
        }

        public decimal MonthlyRevenue
        {
            get => _monthlyRevenue;
            set { _monthlyRevenue = value; OnPropertyChanged(); }
        }

        public decimal MonthlyExpenses
        {
            get => _monthlyExpenses;
            set { _monthlyExpenses = value; OnPropertyChanged(); }
        }

        public decimal NetProfit => MonthlyRevenue - MonthlyExpenses;

        public List<SalesOrder> ActiveSalesOrders => _activeSalesOrders;
        public List<WorkOrder> ActiveWorkOrders => _activeWorkOrders;
        public Dictionary<string, InventoryItem> Inventory => _inventory;
        public List<Employee> Employees => _employees;

        // Methods
        public void AdvanceGameTime()
        {
            GameDay++;
            CurrentGameDate = CurrentGameDate.AddDays(1);

            // Daily processes
            ProcessDailyOperations();

            // Weekly processes
            if (GameDay % 7 == 0)
            {
                GameWeek++;
                ProcessWeeklyOperations();
            }

            // Monthly processes
            if (GameDay % 30 == 0)
            {
                GameMonth++;
                ProcessMonthlyOperations();
            }
        }

        private void ProcessDailyOperations()
        {
            // Production operations
            ProcessProduction();

            // Logistics operations
            ProcessShipments();

            // Sales operations
            ProcessNewOrders();

            // Random events
            TriggerRandomEvents();
        }

        private void ProcessWeeklyOperations()
        {
            // Market demand fluctuations
            UpdateMarketDemand();

            // Competitor actions
            UpdateCompetitorPricing();

            // Employee performance reviews
            UpdateEmployeePerformance();
        }

        private void ProcessMonthlyOperations()
        {
            // Payroll processing
            ProcessPayroll();

            // Financial statements
            GenerateFinancialStatements();

            // Budget reviews
            ReviewDepartmentBudgets();

            // Clear monthly counters
            MonthlyRevenue = 0;
            MonthlyExpenses = 0;
        }

        public bool ProcessTransaction(Transaction transaction)
        {
            if (transaction.Type == TransactionType.Expense)
            {
                if (CashBalance < transaction.Amount)
                {
                    TriggerEvent(new GameEvent
                    {
                        Type = EventType.InsufficientFunds,
                        Message = $"Insufficient funds for {transaction.Description}",
                        Severity = EventSeverity.High
                    });
                    return false;
                }

                CashBalance -= transaction.Amount;
                MonthlyExpenses += transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Revenue)
            {
                CashBalance += transaction.Amount;
                MonthlyRevenue += transaction.Amount;
            }

            return true;
        }

        public SalesOrder CreateSalesOrder(string customerId, List<OrderLine> items)
        {
            var order = new SalesOrder
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                OrderDate = CurrentGameDate,
                Items = items,
                Status = OrderStatus.Pending,
                TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice)
            };

            _activeSalesOrders.Add(order);

            TriggerEvent(new GameEvent
            {
                Type = EventType.NewSalesOrder,
                Message = $"New sales order #{order.Id} - ${order.TotalAmount:N2}",
                Severity = EventSeverity.Info
            });

            return order;
        }

        public WorkOrder CreateWorkOrder(string productId, int quantity)
        {
            var workOrder = new WorkOrder
            {
                Id = Guid.NewGuid().ToString(),
                ProductId = productId,
                Quantity = quantity,
                StartDate = CurrentGameDate,
                Status = WorkOrderStatus.Scheduled,
                RequiredMaterials = GetRequiredMaterials(productId, quantity)
            };

            // Check material availability
            bool materialsAvailable = CheckMaterialAvailability(workOrder.RequiredMaterials);

            if (!materialsAvailable)
            {
                workOrder.Status = WorkOrderStatus.WaitingMaterials;
                TriggerEvent(new GameEvent
                {
                    Type = EventType.MaterialShortage,
                    Message = $"Work Order {workOrder.Id} waiting for materials",
                    Severity = EventSeverity.Medium
                });
            }

            _activeWorkOrders.Add(workOrder);
            return workOrder;
        }

        public void ProcessProduction()
        {
            foreach (var workOrder in _activeWorkOrders.Where(w => w.Status == WorkOrderStatus.InProgress))
            {
                workOrder.ProgressPercentage += 10; // 10% per day (complete in 10 days)

                if (workOrder.ProgressPercentage >= 100)
                {
                    CompleteWorkOrder(workOrder);
                }
            }
        }

        private void CompleteWorkOrder(WorkOrder workOrder)
        {
            workOrder.Status = WorkOrderStatus.Completed;
            workOrder.CompletionDate = CurrentGameDate;

            // Add to finished goods inventory
            if (_inventory.ContainsKey(workOrder.ProductId))
            {
                _inventory[workOrder.ProductId].Quantity += workOrder.Quantity;
            }
            else
            {
                _inventory[workOrder.ProductId] = new InventoryItem
                {
                    ProductId = workOrder.ProductId,
                    Quantity = workOrder.Quantity,
                    LastRestocked = CurrentGameDate
                };
            }

            TriggerEvent(new GameEvent
            {
                Type = EventType.ProductionCompleted,
                Message = $"Work Order {workOrder.Id} completed - {workOrder.Quantity} units produced",
                Severity = EventSeverity.Info
            });
        }

        private void ProcessShipments()
        {
            var readyOrders = _activeSalesOrders
                .Where(o => o.Status == OrderStatus.ReadyToShip)
                .ToList();

            foreach (var order in readyOrders)
            {
                // Check if inventory available
                bool canFulfill = order.Items.All(item =>
                    _inventory.ContainsKey(item.ProductId) &&
                    _inventory[item.ProductId].Quantity >= item.Quantity);

                if (canFulfill)
                {
                    // Deduct inventory
                    foreach (var item in order.Items)
                    {
                        _inventory[item.ProductId].Quantity -= item.Quantity;
                    }

                    order.Status = OrderStatus.Shipped;
                    order.ShipDate = CurrentGameDate;

                    TriggerEvent(new GameEvent
                    {
                        Type = EventType.OrderShipped,
                        Message = $"Order {order.Id} shipped",
                        Severity = EventSeverity.Info
                    });
                }
            }
        }

        private void ProcessNewOrders()
        {
            // Simulate random customer orders based on market demand
            var random = new Random();

            if (random.NextDouble() < 0.3 * (double)_marketDemandMultiplier) // 30% chance per day
            {
                GenerateRandomSalesOrder();
            }
        }

        private void GenerateRandomSalesOrder()
        {
            var random = new Random();
            var products = _productDemand.Keys.ToList();

            if (products.Count == 0) return;

            var selectedProduct = products[random.Next(products.Count)];
            var quantity = random.Next(10, 100);
            var basePrice = 100m; // Base price per unit

            var order = CreateSalesOrder(
                customerId: $"CUST-{random.Next(1000, 9999)}",
                items: new List<OrderLine>
                {
                    new OrderLine
                    {
                        ProductId = selectedProduct,
                        Quantity = quantity,
                        UnitPrice = basePrice
                    }
                }
            );
        }

        private void ProcessPayroll()
        {
            var payrollAmount = _employees.Sum(e => e.MonthlySalary);

            ProcessTransaction(new Transaction
            {
                Type = TransactionType.Expense,
                Amount = payrollAmount,
                Description = "Monthly Payroll",
                Category = "Salaries",
                Date = CurrentGameDate
            });

            TriggerEvent(new GameEvent
            {
                Type = EventType.PayrollProcessed,
                Message = $"Payroll processed: ${payrollAmount:N2}",
                Severity = EventSeverity.Info
            });
        }

        private void UpdateMarketDemand()
        {
            var random = new Random();
            var change = (decimal)(random.NextDouble() * 0.2 - 0.1); // +/- 10%

            _marketDemandMultiplier = Math.Max(0.5m, Math.Min(2.0m, _marketDemandMultiplier + change));

            if (Math.Abs(change) > 0.05m)
            {
                TriggerEvent(new GameEvent
                {
                    Type = EventType.MarketChange,
                    Message = $"Market demand {(change > 0 ? "increased" : "decreased")} by {Math.Abs(change):P1}",
                    Severity = EventSeverity.Info
                });
            }
        }

        private void UpdateCompetitorPricing()
        {
            var random = new Random();
            var change = (decimal)(random.NextDouble() * 0.1 - 0.05); // +/- 5%

            _competitorPriceIndex += change;
        }

        private void UpdateEmployeePerformance()
        {
            var random = new Random();

            foreach (var employee in _employees)
            {
                var performanceChange = random.Next(-5, 10); // Performance can improve more than decline
                employee.PerformanceRating = Math.Max(0, Math.Min(100, employee.PerformanceRating + performanceChange));

                // Satisfaction affects performance
                if (employee.SatisfactionLevel < 50)
                {
                    employee.PerformanceRating -= 2;
                }
            }
        }

        private void CheckCashFlowAlerts()
        {
            if (CashBalance < 100000m)
            {
                TriggerEvent(new GameEvent
                {
                    Type = EventType.LowCashFlow,
                    Message = "Warning: Cash balance is critically low!",
                    Severity = EventSeverity.High
                });
            }
            else if (CashBalance < 250000m)
            {
                TriggerEvent(new GameEvent
                {
                    Type = EventType.LowCashFlow,
                    Message = "Caution: Cash balance is running low",
                    Severity = EventSeverity.Medium
                });
            }
        }

        private void TriggerRandomEvents()
        {
            var random = new Random();

            if (random.NextDouble() < 0.05) // 5% chance per day
            {
                var eventType = random.Next(0, 5);

                switch (eventType)
                {
                    case 0: // Machine breakdown
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.MachineBreakdown,
                            Message = "Machine breakdown in production - delays expected",
                            Severity = EventSeverity.Medium
                        });
                        break;

                    case 1: // Major customer order
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.MajorOrder,
                            Message = "Major customer placed large order!",
                            Severity = EventSeverity.Info
                        });
                        GenerateRandomSalesOrder();
                        break;

                    case 2: // Supplier delay
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.SupplierDelay,
                            Message = "Supplier delivery delayed by 3 days",
                            Severity = EventSeverity.Medium
                        });
                        break;

                    case 3: // Quality issue
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.QualityIssue,
                            Message = "Quality control found defects in recent production",
                            Severity = EventSeverity.High
                        });
                        break;

                    case 4: // Positive market trend
                        _marketDemandMultiplier += 0.1m;
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.MarketOpportunity,
                            Message = "Market conditions favorable - demand increasing!",
                            Severity = EventSeverity.Info
                        });
                        break;
                }
            }
        }

        private Dictionary<string, int> GetRequiredMaterials(string productId, int quantity)
        {
            // Simplified Bill of Materials
            return new Dictionary<string, int>
            {
                { "RM-001", quantity * 2 },  // 2 units of raw material per product
                { "RM-002", quantity * 1 }   // 1 unit of component per product
            };
        }

        private bool CheckMaterialAvailability(Dictionary<string, int> requiredMaterials)
        {
            foreach (var material in requiredMaterials)
            {
                if (!_rawMaterials.ContainsKey(material.Key) ||
                    _rawMaterials[material.Key].Quantity < material.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private void ReviewDepartmentBudgets()
        {
            // Each department gets monthly budget review
            TriggerEvent(new GameEvent
            {
                Type = EventType.BudgetReview,
                Message = "Monthly budget review - department performance assessed",
                Severity = EventSeverity.Info
            });
        }

        private void GenerateFinancialStatements()
        {
            // Calculate financial metrics
            _totalAssets = CashBalance + _inventory.Sum(i => i.Value.Quantity * 50m); // Simplified
            _companyValue = _totalAssets - _totalLiabilities;

            TriggerEvent(new GameEvent
            {
                Type = EventType.FinancialReport,
                Message = $"Monthly report: Revenue ${MonthlyRevenue:N0}, Expenses ${MonthlyExpenses:N0}, Net ${NetProfit:N0}",
                Severity = EventSeverity.Info
            });
        }

        private void TriggerEvent(GameEvent gameEvent)
        {
            OnGameEvent?.Invoke(gameEvent);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Supporting Models

    public class RawMaterial
    {
        public string MaterialId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }



    public class Transaction
    {
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }

    public class PurchaseOrder
    {
        public string Id { get; set; }
        public string SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public List<PurchaseOrderLine> Items { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class PurchaseOrderLine
    {
        public string MaterialId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }

    // Enums

    public enum TransactionType
    {
        Revenue,
        Expense
    }
}