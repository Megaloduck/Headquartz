using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Headquartz.PageModels;

namespace Headquartz.Pages
{
    public partial class SidebarPage : ContentPage
    {
        private readonly SidebarPageModel _vm;

        // Simple content host stack to simulate navigation in the ContentView
        private readonly Stack<Page> _pageStack = new();

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
                if (page == null)
                    return;

                // If no pages yet, set initial
                if (_pageStack.Count == 0)
                {
                    _pageStack.Push(page);
                    ContentHost.Content = page.Content; // Use the Page's Content
                    return;
                }

                // Push new page
                _pageStack.Push(page);
                ContentHost.Content = page.Content; // Use the Page's Content
            });
        }

        // Optional: expose a Back method if needed by UI
        public bool CanGoBack() => _pageStack.Count > 1;

        public void GoBack()
        {
            if (CanGoBack())
            {
                // Pop current
                _pageStack.Pop();
                var top = _pageStack.Peek();
                ContentHost.Content = top.Content; // Use the Page's Content
            }
        }
    }
}
