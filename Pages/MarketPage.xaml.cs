using Headquartz.PageModels;

namespace Headquartz.Pages;

public partial class MarketPage : ContentPage
{
    public MarketPage(MarketPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}