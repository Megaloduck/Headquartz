using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Headquartz
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Start the app at LoginPage
            GoToAsync("//LoginPage");
        }
    }
}
