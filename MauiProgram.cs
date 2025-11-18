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
            .ConfigureFonts(fonts => { });


            // singleton game state
            builder.Services.AddSingleton<GameState>();


            // engine
            builder.Services.AddSingleton<ISimulationEngine, SimulationEngine>(sp =>
            {
                var gs = sp.GetRequiredService<GameState>();
                return new SimulationEngine(gs) { TickRate = TimeSpan.FromSeconds(1) };
            });


            builder.Services.AddSingleton<ISaveService, JsonSaveService>();


            // viewmodels & views
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<MainPage>();


            return builder.Build();
        }
    }
}
