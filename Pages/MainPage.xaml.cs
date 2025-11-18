using Headquartz.Models;
using Headquartz.PageModels;

namespace Headquartz.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel PageModel)
        {
            InitializeComponent();
            BindingContext = PageModel;
        }
    }
}