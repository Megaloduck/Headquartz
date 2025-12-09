using Headquartz.Simulation;
using Headquartz.Services;


namespace Headquartz.Modules
{
    public interface IModule
    {
        // Called for every phase of every tick.
        void ExecutePhase(TickPhase phase, TickContext ctx, SimulationService simulation);
    }
}