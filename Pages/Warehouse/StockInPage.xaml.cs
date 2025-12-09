using Headquartz.PageModels.Warehouse;

namespace Headquartz.Pages.Warehouse;

public partial class StockInPage : ContentPage
{
	public StockInPage (StockInPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
    }
}