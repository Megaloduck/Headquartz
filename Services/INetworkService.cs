using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;
using Headquartz.Modules;

namespace Headquartz.Services
{
    public interface INetworkService
    {
        Task HostGameAsync(int port = 9050);
        Task JoinGameAsync(string hostIp, int port = 9050);
        Task<List<GameSession>> DiscoverSessionsAsync();
        void SendAction(NetworkMessage message);
        event Action<NetworkMessage> OnMessageReceived;
    }
}
