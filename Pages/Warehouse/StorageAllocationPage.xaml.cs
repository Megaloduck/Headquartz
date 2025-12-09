using Headquartz.PageModels.Warehouse;

namespace Headquartz.Pages.Warehouse;

public partial class StorageAllocationPage : ContentPage
{
	public StorageAllocationPage(StorageAllocationPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
    }
}