namespace Headquartz.Pages;

public partial class WarehousePage : ContentPage
{
    public WarehousePage(WarehousePageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}