using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Headquartz
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Switch to a different role (useful for testing or role changes)
        /// </summary>
        
    }
}