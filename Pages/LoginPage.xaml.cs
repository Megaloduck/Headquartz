using Headquartz.Services;

namespace Headquartz.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(RoleService roleService)
    {
        InitializeComponent();
        BindingContext = new LoginPageModel(roleService);
    }
}
