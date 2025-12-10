using Headquartz.Models;
using Headquartz.PageModels;
using Headquartz.Services;

namespace Headquartz.Pages.Dashboard
{
    public partial class OverviewPage : ContentPage
    {
        public OverviewPage(OverviewPageModel pageModel)
        {
            InitializeComponent();
            BindingContext = pageModel;
        }
    }
}