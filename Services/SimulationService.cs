using Headquartz.Models;
using Headquartz.Modules;
using Headquartz.Services.Headquartz.Services;
using Headquartz.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Services
{
    public class SimulationService : IDisposable
    {
        private readonly TickEngine _tickEngine;
        private readonly List<IModule> _modules = new();
        private readonly object _lock = new();

        public GameState GameState { get; }
        public PlayerActionQueue ActionQueue { get; }
        public SimulationService(int tickIntervalMs = 1000)
        {
            _tickEngine = new TickEngine(tickIntervalMs);
            _tickEngine.OnTick += HandleTick;

            GameState = new GameState();
            ActionQueue = new PlayerActionQueue();
        }

        public void RegisterModule(IModule module)
        {
            lock (_lock) { _modules.Add(module); }
        }

        public void Start() => _tickEngine.Start();
        public void Stop() => _tickEngine.Stop();

        private void HandleTick(TickContext ctx)
        {
            // 1) Collect actions phase (actions are expected to be already in the ActionQueue)
            ExecutePhase(TickPhase.CollectActions, ctx);


            // 2) Execute domain phases in defined order
            var orderedPhases = Enum.GetValues(typeof(TickPhase))
            .Cast<TickPhase>()
            .OrderBy(p => (int)p);


            foreach (var phase in orderedPhases)
            {
                ExecutePhase(phase, ctx);
            }


            // 3) After all modules processed, you may do persistence or broadcast here
            // e.g., DataService.Save(GameState);
        }
        private void ExecutePhase(TickPhase phase, TickContext ctx)
        {
            // Modules may choose to ignore phases they don't implement
            lock (_lock)
            {
                foreach (var module in _modules)
                {
                    try
                    {
                        module.ExecutePhase(phase, ctx, this);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Replace with structured logging
                        Console.Error.WriteLine($"Module {module.GetType().Name} failed on phase {phase}: {ex}");
                    }
                }
            }
        }

        public void Dispose()
        {
            _tickEngine.Dispose();
        }
    }
}
