// Models/GameState.cs - Enhanced for MonsoonSIM-like tick mechanism
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using Headquartz.Models.HumanResource;
using Headquartz.Models.Production;
using Headquartz.Models.Sales;
using Headquartz.Models.Warehouse;

namespace Headquartz.Models
{
    /// <summary>
    /// Central game state with enhanced tick-based processing
    /// Synchronized across all players in multiplayer mode
    /// </summary>
    public class GameState : INotifyPropertyChanged
    {
        private static GameState _instance;
        public static GameState Instance => _instance ??= new GameState();

        #region Time Management

        private DateTime _currentGameDate = new DateTime(2025, 1, 1);
        private int _gameDay = 1;
        private int _gameWeek = 1;
        private int _gameMonth = 1;
        private int _gameYear = 1;
        private int _gameQuarter = 1;

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

        public int GameQuarter
        {
            get => _gameQuarter;
            set { _gameQuarter = value; OnPropertyChanged(); }
        }

        #endregion

        #region Financial Metrics

        private decimal _cashBalance = 1000000m;
        private decimal _monthlyRevenue = 0m;
        private decimal _monthlyExpenses = 0m;
        private decimal _totalAssets = 1000000m;
        private decimal _totalLiabilities = 0m;
        private decimal _quarterlyRevenue = 0m;
        private decimal _quarterlyExpenses = 0m;
        private decimal _yearlyRevenue = 0m;
        private decimal _yearlyExpenses = 0m;

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

        public decimal QuarterlyRevenue
        {
            get => _quarterlyRevenue;
            set { _quarterlyRevenue = value; OnPropertyChanged(); }
        }

        public decimal QuarterlyExpenses
        {
            get => _quarterlyExpenses;
            set { _quarterlyExpenses = value; OnPropertyChanged(); }
        }

        public decimal YearlyRevenue
        {
            get => _yearlyRevenue;
            set { _yearlyRevenue = value; OnPropertyChanged(); }
        }

        public decimal YearlyExpenses
        {
            get => _yearlyExpenses;
            set { _yearlyExpenses = value; OnPropertyChanged(); }
        }

        public decimal NetProfit => MonthlyRevenue - MonthlyExpenses;
        public decimal QuarterlyProfit => QuarterlyRevenue - QuarterlyExpenses;
        public decimal YearlyProfit => YearlyRevenue - YearlyExpenses;

        #endregion

        #region Company Performance

        private decimal _companyValue = 1000000m;
        private int _customerSatisfaction = 80;
        private int _employeeSatisfaction = 75;
        private int _marketShare = 5;
        private decimal _productionEfficiency = 100m;
        private decimal _qualityScore = 95m;

        public decimal CompanyValue
        {
            get => _companyValue;
            set { _companyValue = value; OnPropertyChanged(); }
        }

        public int CustomerSatisfaction
        {
            get => _customerSatisfaction;
            set { _customerSatisfaction = Math.Max(0, Math.Min(100, value)); OnPropertyChanged(); }
        }

        public int EmployeeSatisfaction
        {
            get => _employeeSatisfaction;
            set { _employeeSatisfaction = Math.Max(0, Math.Min(100, value)); OnPropertyChanged(); }
        }

        public int MarketShare
        {
            get => _marketShare;
            set { _marketShare = Math.Max(0, Math.Min(100, value)); OnPropertyChanged(); }
        }

        public decimal ProductionEfficiency
        {
            get => _productionEfficiency;
            set { _productionEfficiency = value; OnPropertyChanged(); }
        }

        public decimal QualityScore
        {
            get => _qualityScore;
            set { _qualityScore = value; OnPropertyChanged(); }
        }

        #endregion

        #region Collections

        private Dictionary<string, InventoryItem> _inventory = new();
        private Dictionary<string, RawMaterial> _rawMaterials = new();
        private List<SalesOrder> _activeSalesOrders = new();
        private List<WorkOrder> _activeWorkOrders = new();
        private Queue<PurchaseOrder> _pendingPurchaseOrders = new();
        private List<Employee> _employees = new();
        private List<GameEvent> _eventHistory = new();

        public Dictionary<string, InventoryItem> Inventory => _inventory;
        public Dictionary<string, RawMaterial> RawMaterials => _rawMaterials;
        public List<SalesOrder> ActiveSalesOrders => _activeSalesOrders;
        public List<WorkOrder> ActiveWorkOrders => _activeWorkOrders;
        public List<Employee> Employees => _employees;
        public List<GameEvent> EventHistory => _eventHistory;

        #endregion

        #region Market Conditions

        private decimal _marketDemandMultiplier = 1.0m;
        private decimal _competitorPriceIndex = 1.0m;
        private Dictionary<string, decimal> _productDemand = new();

        public decimal MarketDemandMultiplier
        {
            get => _marketDemandMultiplier;
            set { _marketDemandMultiplier = value; OnPropertyChanged(); }
        }

        #endregion

        #region Events

        public event Action<GameEvent> OnGameEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Tick Processing Methods

        /// <summary>
        /// Main tick method - advances game by 1 day
        /// </summary>
        public void AdvanceGameTime()
        {
            GameDay++;
            CurrentGameDate = CurrentGameDate.AddDays(1);

            // Daily processes
            ProcessDailyOperations();

            // Weekly processes (every 7 days)
            if (GameDay % 7 == 0)
            {
                GameWeek++;
                ProcessWeeklyOperations();
            }

            // Monthly processes (every 30 days)
            if (GameDay % 30 == 0)
            {
                GameMonth++;
                ProcessMonthlyOperations();
            }

            // Quarterly processes (every 90 days)
            if (GameDay % 90 == 0)
            {
                GameQuarter++;
                ProcessQuarterlyOperations();
            }

            // Yearly processes (every 360 days)
            if (GameDay % 360 == 0)
            {
                GameYear++;
                ProcessYearlyOperations();
            }
        }

        #endregion

        #region Daily Operations

        private void ProcessDailyOperations()
        {
            // Production operations
            ProcessProduction();

            // Logistics operations
            ProcessShipments();

            // Sales operations
            ProcessNewOrders();

            // Employee productivity
            ProcessEmployeeDailyWork();

            // Quality control
            ProcessQualityChecks();

            // Random events (5% chance per day)
            TriggerRandomEvents();

            // Update performance metrics
            UpdateDailyMetrics();
        }

        private void ProcessEmployeeDailyWork()
        {
            foreach (var employee in _employees)
            {
                // Productivity impacts performance
                if (employee.SatisfactionLevel > 80)
                {
                    employee.PerformanceRating = Math.Min(100, employee.PerformanceRating + 1);
                }
                else if (employee.SatisfactionLevel < 40)
                {
                    employee.PerformanceRating = Math.Max(0, employee.PerformanceRating - 1);
                }
            }
        }

        private void ProcessQualityChecks()
        {
            // Random quality variations
            var random = new Random();
            var qualityChange = (decimal)(random.NextDouble() * 2 - 1); // ±1%
            QualityScore = Math.Max(0, Math.Min(100, QualityScore + qualityChange));
        }

        private void UpdateDailyMetrics()
        {
            // Update company value based on assets
            _totalAssets = CashBalance + _inventory.Sum(i => i.Value.Quantity * 50m);
            CompanyValue = _totalAssets - _totalLiabilities;

            // Update production efficiency
            if (_activeWorkOrders.Any())
            {
                var avgProgress = _activeWorkOrders
                    .Where(w => w.Status == WorkOrderStatus.InProgress)
                    .Average(w => (decimal?)w.ProgressPercentage) ?? 100m;
                ProductionEfficiency = avgProgress;
            }
        }

        #endregion

        #region Weekly Operations

        private void ProcessWeeklyOperations()
        {
            // Market demand fluctuations
            UpdateMarketDemand();

            // Competitor actions
            UpdateCompetitorPricing();

            // Employee performance reviews
            UpdateEmployeePerformance();

            // Customer satisfaction updates
            UpdateCustomerSatisfaction();

            TriggerEvent(new GameEvent
            {
                Type = EventType.MarketChange,
                Message = $"Week {GameWeek} completed - Market analysis updated",
                Severity = EventSeverity.Info
            });
        }

        private void UpdateCustomerSatisfaction()
        {
            var random = new Random();

            // Quality impacts satisfaction
            if (QualityScore > 90)
            {
                CustomerSatisfaction += random.Next(1, 3);
            }
            else if (QualityScore < 70)
            {
                CustomerSatisfaction -= random.Next(1, 3);
            }

            // Delivery performance impacts satisfaction
            var onTimeDeliveries = _activeSalesOrders
                .Count(o => o.Status == OrderStatus.Shipped);
            if (onTimeDeliveries > 5)
            {
                CustomerSatisfaction += 1;
            }
        }

        #endregion

        #region Monthly Operations

        private void ProcessMonthlyOperations()
        {
            // Payroll processing
            ProcessPayroll();

            // Financial statements
            GenerateFinancialStatements();

            // Budget reviews
            ReviewDepartmentBudgets();

            // Accumulate to quarterly
            QuarterlyRevenue += MonthlyRevenue;
            QuarterlyExpenses += MonthlyExpenses;

            // Clear monthly counters
            MonthlyRevenue = 0;
            MonthlyExpenses = 0;

            TriggerEvent(new GameEvent
            {
                Type = EventType.FinancialReport,
                Message = $"Month {GameMonth} closed - Financial reports generated",
                Severity = EventSeverity.Info
            });
        }

        #endregion

        #region Quarterly Operations

        private void ProcessQuarterlyOperations()
        {
            // Quarterly performance review
            TriggerEvent(new GameEvent
            {
                Type = EventType.BudgetReview,
                Message = $"Q{GameQuarter} Results: Revenue ${QuarterlyRevenue:N0}, Profit ${QuarterlyProfit:N0}",
                Severity = EventSeverity.Info
            });

            // Strategic planning for next quarter
            AdjustMarketStrategy();

            // Accumulate to yearly
            YearlyRevenue += QuarterlyRevenue;
            YearlyExpenses += QuarterlyExpenses;

            // Clear quarterly counters
            QuarterlyRevenue = 0;
            QuarterlyExpenses = 0;
        }

        private void AdjustMarketStrategy()
        {
            // AI-driven market strategy adjustments
            if (MarketShare < 10 && CashBalance > 500000m)
            {
                TriggerEvent(new GameEvent
                {
                    Type = EventType.MarketOpportunity,
                    Message = "Market opportunity detected - Consider increasing marketing budget",
                    Severity = EventSeverity.Info
                });
            }
        }

        #endregion

        #region Yearly Operations

        private void ProcessYearlyOperations()
        {
            // Annual report
            TriggerEvent(new GameEvent
            {
                Type = EventType.FinancialReport,
                Message = $"Year {GameYear} Complete! Annual Revenue: ${YearlyRevenue:N0}, Profit: ${YearlyProfit:N0}",
                Severity = EventSeverity.Info
            });

            // Employee annual reviews
            foreach (var employee in _employees)
            {
                if (employee.PerformanceRating > 85)
                {
                    employee.MonthlySalary *= 1.05m; // 5% raise for high performers
                }
            }

            // Clear yearly counters
            YearlyRevenue = 0;
            YearlyExpenses = 0;
        }

        #endregion

        #region Production & Logistics (Existing Enhanced Methods)

        public void ProcessProduction()
        {
            foreach (var workOrder in _activeWorkOrders.Where(w => w.Status == WorkOrderStatus.InProgress).ToList())
            {
                // Progress based on employee skill and satisfaction
                var productionRate = CalculateProductionRate();
                workOrder.ProgressPercentage += (int)(10 * productionRate);

                if (workOrder.ProgressPercentage >= 100)
                {
                    CompleteWorkOrder(workOrder);
                }
            }

            // Start scheduled work orders
            foreach (var workOrder in _activeWorkOrders.Where(w => w.Status == WorkOrderStatus.Scheduled).ToList())
            {
                if (CheckMaterialAvailability(workOrder.RequiredMaterials))
                {
                    workOrder.Status = WorkOrderStatus.InProgress;
                    ConsumeRawMaterials(workOrder.RequiredMaterials);
                }
            }
        }

        private decimal CalculateProductionRate()
        {
            if (!_employees.Any()) return 1.0m;

            var avgPerformance = (decimal)_employees.Average(e => e.PerformanceRating) / 100m;
            var avgSatisfaction = (decimal)_employees.Average(e => e.SatisfactionLevel) / 100m;

            return (avgPerformance + avgSatisfaction) / 2m;
        }

        private void ConsumeRawMaterials(Dictionary<string, int> requiredMaterials)
        {
            foreach (var material in requiredMaterials)
            {
                if (_rawMaterials.ContainsKey(material.Key))
                {
                    _rawMaterials[material.Key].Quantity -= material.Value;
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
                        Message = $"Order {order.Id} shipped - ${order.TotalAmount:N2}",
                        Severity = EventSeverity.Info
                    });

                    // Customer satisfaction boost
                    CustomerSatisfaction += 1;
                }
            }
        }

        #endregion

        #region Sales & Orders (Enhanced)

        private void ProcessNewOrders()
        {
            var random = new Random();
            double orderChance = 0.3 * (double)_marketDemandMultiplier * (CustomerSatisfaction / 100.0);

            if (random.NextDouble() < orderChance)
            {
                GenerateRandomSalesOrder();
            }
        }

        private void GenerateRandomSalesOrder()
        {
            var random = new Random();

            // Ensure we have product demand data
            if (_productDemand.Count == 0)
            {
                _productDemand["P-001"] = 1.0m;
                _productDemand["P-002"] = 1.2m;
            }

            var products = _productDemand.Keys.ToList();
            var selectedProduct = products[random.Next(products.Count)];
            var quantity = random.Next(10, 100);
            var basePrice = 100m * _competitorPriceIndex;

            CreateSalesOrder(
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
                Message = $"New order from {customerId} - ${order.TotalAmount:N2}",
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

            _activeWorkOrders.Add(workOrder);
            return workOrder;
        }

        #endregion

        #region Financial Operations

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

        private void ProcessPayroll()
        {
            var payrollAmount = _employees.Sum(e => e.MonthlySalary);

            if (ProcessTransaction(new Transaction
            {
                Type = TransactionType.Expense,
                Amount = payrollAmount,
                Description = "Monthly Payroll",
                Category = "Salaries",
                Date = CurrentGameDate
            }))
            {
                TriggerEvent(new GameEvent
                {
                    Type = EventType.PayrollProcessed,
                    Message = $"Payroll processed: ${payrollAmount:N2}",
                    Severity = EventSeverity.Info
                });

                // Employee satisfaction boost after payroll
                foreach (var employee in _employees)
                {
                    employee.SatisfactionLevel = Math.Min(100, employee.SatisfactionLevel + 2);
                }
            }
        }

        #endregion

        #region Market & Competition

        private void UpdateMarketDemand()
        {
            var random = new Random();
            var change = (decimal)(random.NextDouble() * 0.2 - 0.1); // ±10%

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
            var change = (decimal)(random.NextDouble() * 0.1 - 0.05); // ±5%
            _competitorPriceIndex += change;
        }

        private void UpdateEmployeePerformance()
        {
            var random = new Random();

            foreach (var employee in _employees)
            {
                var performanceChange = random.Next(-5, 10);
                employee.PerformanceRating = Math.Max(0, Math.Min(100, employee.PerformanceRating + performanceChange));

                if (employee.SatisfactionLevel < 50)
                {
                    employee.PerformanceRating -= 2;
                }
            }
        }

        #endregion

        #region Random Events

        private void TriggerRandomEvents()
        {
            var random = new Random();

            if (random.NextDouble() < 0.05) // 5% chance
            {
                var eventType = random.Next(0, 5);

                switch (eventType)
                {
                    case 0:
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.MachineBreakdown,
                            Message = "Production equipment malfunction - efficiency reduced",
                            Severity = EventSeverity.Medium
                        });
                        ProductionEfficiency *= 0.8m;
                        break;

                    case 1:
                        GenerateRandomSalesOrder();
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.MajorOrder,
                            Message = "Major customer placed large order!",
                            Severity = EventSeverity.Info
                        });
                        break;

                    case 2:
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.SupplierDelay,
                            Message = "Supplier delivery delayed",
                            Severity = EventSeverity.Medium
                        });
                        break;

                    case 3:
                        QualityScore -= 5;
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.QualityIssue,
                            Message = "Quality control found defects",
                            Severity = EventSeverity.High
                        });
                        break;

                    case 4:
                        _marketDemandMultiplier += 0.1m;
                        TriggerEvent(new GameEvent
                        {
                            Type = EventType.MarketOpportunity,
                            Message = "Favorable market conditions!",
                            Severity = EventSeverity.Info
                        });
                        break;
                }
            }
        }

        #endregion

        #region Helper Methods

        private void CheckCashFlowAlerts()
        {
            if (CashBalance < 100000m)
            {
                TriggerEvent(new GameEvent
                {
                    Type = EventType.LowCashFlow,
                    Message = "CRITICAL: Cash balance critically low!",
                    Severity = EventSeverity.High
                });
            }
            else if (CashBalance < 250000m)
            {
                TriggerEvent(new GameEvent
                {
                    Type = EventType.LowCashFlow,
                    Message = "Warning: Cash balance running low",
                    Severity = EventSeverity.Medium
                });
            }
        }

        private Dictionary<string, int> GetRequiredMaterials(string productId, int quantity)
        {
            return new Dictionary<string, int>
            {
                { "RM-001", quantity * 2 },
                { "RM-002", quantity * 1 }
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
            TriggerEvent(new GameEvent
            {
                Type = EventType.BudgetReview,
                Message = "Department budget review completed",
                Severity = EventSeverity.Info
            });
        }

        private void GenerateFinancialStatements()
        {
            _totalAssets = CashBalance + _inventory.Sum(i => i.Value.Quantity * 50m);
            _companyValue = _totalAssets - _totalLiabilities;
        }

        private void TriggerEvent(GameEvent gameEvent)
        {
            _eventHistory.Add(gameEvent);

            // Keep only last 100 events
            if (_eventHistory.Count > 100)
            {
                _eventHistory.RemoveAt(0);
            }

            OnGameEvent?.Invoke(gameEvent);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
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