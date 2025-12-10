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
        event Action<GameState> OnTicked;

        void Start();
        void Stop();
        void SetSpeed(double multiplier);
        bool IsRunning { get; }
        double CurrentSpeed { get; }
    }
}
