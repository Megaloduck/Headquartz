using Headquartz.PageModels.Warehouse;

namespace Headquartz.Pages.Warehouse;

public partial class ShipmentsPage : ContentPage
{
	public ShipmentsPage(ShipmentsPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
    }
}