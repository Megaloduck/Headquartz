using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Headquartz.Pages;
using Headquartz.Services;
using Headquartz.Pages.CEO;
using Headquartz.Pages.Dashboard;
using Headquartz.Pages.Finance;
using Headquartz.Pages.HumanResource;
using Headquartz.Pages.Logistics;
using Headquartz.Pages.Marketing;
using Headquartz.Pages.Production;
using Headquartz.Pages.Sales;
using Headquartz.Pages.System;
using Headquartz.Pages.Warehouse;

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
            var sidebarPage = services.GetRequiredService<SidebarFinancePage>();

            return new Window(sidebarPage);
        }
    }
}
