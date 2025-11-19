namespace Headquartz.Pages;

public partial class InventoryPage : ContentPage
{
    public InventoryPage(InventoryPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}