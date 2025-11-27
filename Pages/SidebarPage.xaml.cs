using Microsoft.Maui.Controls;
using Headquartz.PageModels;

namespace Headquartz.Pages
{
    public partial class SidebarPage : ContentPage
    {
        private readonly SidebarPageModel _vm;

        public SidebarPage(SidebarPageModel vm)
        {
            InitializeComponent();

            _vm = vm;
            _vm.NavigateAction = NavigateToPage;

            BindingContext = _vm;
        }

        private async void NavigateToPage(Page page)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (ContentHost.Navigation.NavigationStack.Count == 0)
                {
                    // First page
                    ContentHost.Navigation.InsertPageBefore(page, ContentHost.CurrentPage);
                    await ContentHost.PopAsync(false);
                }
                else
                {
                    await ContentHost.PushAsync(page);
                }
            });
        }
    }
}
