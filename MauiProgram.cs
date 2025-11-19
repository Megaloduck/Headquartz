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


            // singleton game state
            builder.Services.AddSingleton<GameState>();

            // engine
            builder.Services.AddSingleton<ISimulationEngine, SimulationEngine>();
            builder.Services.AddSingleton<ISaveService, JsonSaveService>();

            // PageModels & Pages
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<MarketPageModel>();
            builder.Services.AddTransient<MarketPage>();
            builder.Services.AddSingleton<InventoryPageModel>();
            builder.Services.AddSingleton<InventoryPage>();


            return builder.Build();
        }
    }
}
