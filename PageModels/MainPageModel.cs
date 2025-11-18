using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Headquartz.Models;
using Headquartz.Services;

namespace Headquartz.PageModels
{
    public partial class MainPageModel : INotifyPropertyChanged
    {
        private readonly ISimulationEngine _engine;
        private readonly GameState _state;


        public MainPageModel(ISimulationEngine engine, GameState state)
        {
            _engine = engine;
            _state = state;
            _engine.OnTicked += s =>
            {
                // UI thread marshal
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SimTime = s.SimTime.ToLocalTime().ToString("HH:mm:ss");
                    Cash = s.Company.Cash.ToString("C0");
                });
            };
        }


        private string _simTime = "-";
        public string SimTime { get => _simTime; set { _simTime = value; OnPropertyChanged(); } }


        private string _cash = "-";
        public string Cash { get => _cash; set { _cash = value; OnPropertyChanged(); } }


        public void Start() => _engine.Start();
        public void Stop() => _engine.Stop();


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}