using System;
using Headquartz.Models;

namespace Headquartz.Services
{
    public interface ISimulationEngine
    {
        // Events
        event Action<GameState> OnTicked;
        event Action<GameState> OnDayTicked;
        event Action<GameState> OnWeekTicked;
        event Action<GameState> OnMonthTicked;
        event Action<GameState> OnQuarterTicked;
        event Action<GameState> OnYearTicked;
        event Action<SimulationPhase> OnPhaseChanged;

        // Control Methods
        void Start();
        void Stop();
        void SetSpeed(double multiplier);
        void Reset();

        // Properties
        bool IsRunning { get; }
        double CurrentSpeed { get; }
        SimulationPhase CurrentPhase { get; }
        int TicksProcessed { get; }
        DateTime LastTickTime { get; }

        // Statistics
        SimulationStatistics GetStatistics();
    }
}