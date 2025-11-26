using Headquartz.Models;
using Headquartz.PageModels;

namespace Headquartz.Pages
{
    public partial class OverviewPage : ContentPage
    {
        public OverviewPage(OverviewPageModel PageModel)
        {
            InitializeComponent();
            BindingContext = PageModel;
        }
    }
}