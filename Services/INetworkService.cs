using Headquartz.Models;

namespace Headquartz.Services
{
    public interface INetworkService
    {
        bool IsHost { get; }
        bool IsConnected { get; }

        event Action<GameState> OnGameStateReceived;
        event Action<string> OnLog;

        void StartHost(int port);
        void Connect(string ip, int port);
        void Stop();
        void PollEvents();
        void BroadcastGameState(GameState state);
    }
}
