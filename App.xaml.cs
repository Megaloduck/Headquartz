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

            // Create AppShell with the required dependency
            return new Window(new AppShell(roleService));
        }
    }
}