using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Headquartz.Models; 
using Headquartz.Services;

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

            // Singleton game state
            builder.Services.AddSingleton<GameState>();

            // Services
            builder.Services.AddSingleton<ISimulationEngine, SimulationEngine>();
            builder.Services.AddSingleton<ISaveService, JsonSaveService>();
            builder.Services.AddSingleton<RoleService>();

            // PageModels & Pages
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<DashboardPageModel>();
            builder.Services.AddTransient<DashboardPage>();

            builder.Services.AddTransient<MarketPageModel>();
            builder.Services.AddTransient<MarketPage>();

            builder.Services.AddTransient<InventoryPageModel>();
            builder.Services.AddTransient<InventoryPage>();

            builder.Services.AddTransient<FinancePage>();
            builder.Services.AddTransient<HumanResourcePage>();

            // Seed data
            var app = builder.Build();
            var state = app.Services.GetRequiredService<GameState>();
            SeedService.Seed(state);

            return app;
        }
    }
}
