using Headquartz.PageModels.Warehouse;

namespace Headquartz.Pages.Warehouse;

public partial class InventoryPage : ContentPage
{
    public InventoryPage(InventoryPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}