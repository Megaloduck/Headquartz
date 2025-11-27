using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Headquartz.Pages;
using Headquartz.Services;

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
            // Get services from the DI container
            var services = Handler?.MauiContext?.Services;

            if (services == null)
            {
                throw new InvalidOperationException("Services not available");
            }

            var roleService = services.GetRequiredService<RoleService>();

            // Create SidebarPage with required dependencies
            var sidebarPage = new SidebarPage();

            return new Window(sidebarPage);
        }
    }
}