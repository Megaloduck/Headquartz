using Headquartz.PageModels;

namespace Headquartz.Pages;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}