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
        GameState State { get; } // Add this
        void Start();
        void Stop();
        event Action<GameState>? OnTicked;
    }
}
