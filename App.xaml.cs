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
            // Get RoleService from the DI container
            var roleService = Handler?.MauiContext?.Services.GetRequiredService<RoleService>();

            // Store services for later access
            Headquartz.Pages.MauiProgram.Services = Handler?.MauiContext?.Services;

            // Use MainLayout instead of AppShell
            return new Window(new SidebarPage(roleService));
        }
    }
}