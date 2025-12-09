using Headquartz.PageModels.Warehouse;

namespace Headquartz.Pages.Warehouse;

public partial class WarehouseReportsPage : ContentPage
{
    public WarehouseReportsPage (WarehouseReportsPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}