using Headquartz.PageModels.Warehouse;

namespace Headquartz.Pages.Warehouse;

public partial class StockOutPage : ContentPage
{
	public StockOutPage(StockOutPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
    }
}