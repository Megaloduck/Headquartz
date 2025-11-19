using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Headquartz.Services;
using Headquartz.Models;

namespace Headquartz.PageModels
{
    public class DashboardPageModel : BasePageModel
    {
        private readonly ISimulationEngine _engine;

        public string GameTimeString => _engine.State.SimTime.ToString();

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public DashboardPageModel(ISimulationEngine engine)
        {
            _engine = engine;
            StartCommand = new Command(() => _engine.Start());
            StopCommand = new Command(() => _engine.Stop());

            _engine.OnTicked += _ => OnPropertyChanged(nameof(GameTimeString));
        }
    }
}
