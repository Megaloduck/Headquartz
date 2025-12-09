using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.Services
{
    namespace Headquartz.Services
    {
        // Simple thread-safe queue of actions (you can replace PlayerAction with richer model)
        public record PlayerAction(string PlayerId, string ActionType, object? Payload);
        public class PlayerActionQueue
        {
            private readonly ConcurrentQueue<PlayerAction> _queue = new();

            public void Enqueue(PlayerAction action) => _queue.Enqueue(action);
            public IEnumerable<PlayerAction> DequeueAll()
            {
                while (_queue.TryDequeue(out var a))
                {
                    yield return a;
                }
            }
            public int Count => _queue.Count;
        }
    }
}
