using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Services
{
    public interface ISimulationEngine
    {
        bool IsRunning { get; }
        TimeSpan TickRate { get; set; }
        void Start();
        void Stop();
        event Action<GameState>? OnTicked; // viewmodels can subscribe
    }
}
