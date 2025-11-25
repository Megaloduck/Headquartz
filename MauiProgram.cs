using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Headquartz.Models;
using Headquartz.Services;
using Headquartz.Pages;
using Headquartz.PageModels;

namespace Headquartz
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts => { });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Core Game State - Singleton
            builder.Services.AddSingleton<GameState>();

            // Core Services - Singletons
            builder.Services.AddSingleton<ISimulationEngine, SimulationEngine>();
            builder.Services.AddSingleton<ISaveService, JsonSaveService>();
            builder.Services.AddSingleton<RoleService>(); // ✅ IMPORTANT: Must be registered

            // Dashboard
            builder.Services.AddTransient<DashboardPageModel>();
            builder.Services.AddTransient<DashboardPage>();

            // Main Page
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<MainPage>();

            // Market
            builder.Services.AddTransient<MarketPageModel>();
            builder.Services.AddTransient<MarketPage>();

            // Warehouse
            builder.Services.AddTransient<WarehousePageModel>();
            builder.Services.AddTransient<WarehousePage>();

            // Finance
            builder.Services.AddTransient<FinancePage>();

            // Human Resources
            builder.Services.AddTransient<HumanResourcePage>();

            var app = builder.Build();

            // Seed initial game data
            var state = app.Services.GetRequiredService<GameState>();
            SeedService.Seed(state);

            return app;
        }
    }
}