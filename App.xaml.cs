using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Headquartz.Pages;
using Headquartz.Services;
using Headquartz.Pages.CEO;

namespace Headquartz
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Get services from DI container
            var services = Handler?.MauiContext?.Services
                ?? throw new InvalidOperationException("Services not available");

            // 👍 Resolve SidebarPage through DI (IMPORTANT)
            var sidebarPage = services.GetRequiredService<SidebarCEOPage>();

            return new Window(sidebarPage);
        }
    }
}
