using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Headquartz.Models;
using Headquartz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Headquartz.PageModels
{
    public class MarketPageModel : BasePageModel
    {
        private readonly GameState _state;
        private readonly ISimulationEngine _engine;

        public int Demand => _state.Market.Demand;
        public float Price => _state.Market.Price;
        public string TrendDescription => _state.Market.TrendDescription;

        private string _forecast = "No forecast yet";
        public string Forecast
        {
            get => _forecast;
            set => SetProperty(ref _forecast, value);
        }

        public ICommand RefreshForecastCommand { get; }

        public MarketPageModel(GameState state, ISimulationEngine engine)
        {
            _state = state;
            _engine = engine;

            RefreshForecastCommand = new Command(GenerateForecast);

            _engine.OnTicked += s => RefreshValues();
        }

        private void RefreshValues()
        {
            OnPropertyChanged(nameof(Demand));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(TrendDescription));
        }

        private void GenerateForecast()
        {
            Forecast = MarketModule.GenerateForecast(_state);
        }
    }
}
