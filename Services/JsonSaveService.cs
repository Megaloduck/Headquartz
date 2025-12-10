using Headquartz.Models;
using Headquartz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Headquartz.Services
{
    public class JsonSaveService : ISaveService
    {
        private readonly JsonSerializerOptions _options;

        public JsonSaveService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        public async Task SaveAsync(GameState state, string path)
        {
            try
            {
                // Create save data object with only serializable properties
                var saveData = new GameStateSaveData
                {
                    CurrentGameDate = state.CurrentGameDate,
                    GameDay = state.GameDay,
                    GameWeek = state.GameWeek,
                    GameMonth = state.GameMonth,
                    GameYear = state.GameYear,
                    CashBalance = state.CashBalance,
                    MonthlyRevenue = state.MonthlyRevenue,
                    MonthlyExpenses = state.MonthlyExpenses,

                    // Serialize collections
                    InventoryItems = state.Inventory.Select(kvp => new InventoryItemSaveData
                    {
                        ProductId = kvp.Key,
                        Quantity = kvp.Value.Quantity,
                        LastRestocked = kvp.Value.LastRestocked,
                        ReorderLevel = kvp.Value.ReorderLevel,
                        ReorderQuantity = kvp.Value.ReorderQuantity
                    }).ToList(),

                    SalesOrders = state.ActiveSalesOrders.ToList(),
                    WorkOrders = state.ActiveWorkOrders.ToList(),
                    Employees = state.Employees.ToList()
                };

                string json = JsonSerializer.Serialize(saveData, _options);

                // Ensure directory exists
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(path, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save game: {ex.Message}", ex);
            }
        }

        public async Task<GameState> LoadAsync(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("Save file not found", path);
                }

                string json = await File.ReadAllTextAsync(path);
                var saveData = JsonSerializer.Deserialize<GameStateSaveData>(json, _options);

                if (saveData == null)
                {
                    throw new Exception("Failed to deserialize save data");
                }

                // Create new GameState and populate it
                var state = new GameState();
                state.CurrentGameDate = saveData.CurrentGameDate;
                state.GameDay = saveData.GameDay;
                state.GameWeek = saveData.GameWeek;
                state.GameMonth = saveData.GameMonth;
                state.GameYear = saveData.GameYear;
                state.CashBalance = saveData.CashBalance;
                state.MonthlyRevenue = saveData.MonthlyRevenue;
                state.MonthlyExpenses = saveData.MonthlyExpenses;

                // Restore inventory
                if (saveData.InventoryItems != null)
                {
                    foreach (var item in saveData.InventoryItems)
                    {
                        state.Inventory[item.ProductId] = new Models.Warehouse.InventoryItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            LastRestocked = item.LastRestocked,
                            ReorderLevel = item.ReorderLevel,
                            ReorderQuantity = item.ReorderQuantity
                        };
                    }
                }

                // Restore orders and employees
                if (saveData.SalesOrders != null)
                {
                    state.ActiveSalesOrders.Clear();
                    state.ActiveSalesOrders.AddRange(saveData.SalesOrders);
                }

                if (saveData.WorkOrders != null)
                {
                    state.ActiveWorkOrders.Clear();
                    state.ActiveWorkOrders.AddRange(saveData.WorkOrders);
                }

                if (saveData.Employees != null)
                {
                    state.Employees.Clear();
                    state.Employees.AddRange(saveData.Employees);
                }

                return state;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load game: {ex.Message}", ex);
            }
        }
    }

    // Save data transfer objects
    public class GameStateSaveData
    {
        public DateTime CurrentGameDate { get; set; }
        public int GameDay { get; set; }
        public int GameWeek { get; set; }
        public int GameMonth { get; set; }
        public int GameYear { get; set; }
        public decimal CashBalance { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public List<InventoryItemSaveData> InventoryItems { get; set; }
        public List<Models.Sales.SalesOrder> SalesOrders { get; set; }
        public List<Models.Production.WorkOrder> WorkOrders { get; set; }
        public List<Models.HumanResource.Employee> Employees { get; set; }
    }

    public class InventoryItemSaveData
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastRestocked { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
    }
}