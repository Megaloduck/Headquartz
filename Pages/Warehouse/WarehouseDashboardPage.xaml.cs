using Headquartz.PageModels.Warehouse;

namespace Headquartz.Pages.Warehouse;

public partial class WarehouseDashboardPage : ContentPage
{
    public WarehouseDashboardPage(WarehouseDashboardPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}