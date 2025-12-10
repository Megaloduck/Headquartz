using LiteNetLib;
using LiteNetLib.Utils;
using Headquartz.Models;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;

namespace Headquartz.Services.Networking
{
    public class NetworkService : INetworkService
    {
        private EventBasedNetListener _listener;
        private NetManager _netManager;
        private NetPeer? _serverPeer; // For client
        
        public bool IsHost { get; private set; }
        public bool IsConnected => _netManager != null && _netManager.IsRunning;

        // Events
        public event Action<GameState>? OnGameStateReceived;
        public event Action<string>? OnLog;

        public NetworkService()
        {
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener);

            _listener.NetworkReceiveEvent += OnNetworkReceive;
            _listener.PeerConnectedEvent += OnPeerConnected;
            _listener.PeerDisconnectedEvent += OnPeerDisconnected;
        }

        public void StartHost(int port)
        {
            if (IsConnected) Stop();

            IsHost = true;
            _netManager.Start(port);
            Log($"Host started on port {port}");
        }

        public void Connect(string ip, int port)
        {
            if (IsConnected) Stop();

            IsHost = false;
            _netManager.Start();
            _serverPeer = _netManager.Connect(ip, port, "HeadquartzKey");
            Log($"Connecting to {ip}:{port}...");
        }

        public void Stop()
        {
            _netManager.Stop();
            IsHost = false;
            _serverPeer = null;
            Log("Network stopped.");
        }

        public void PollEvents()
        {
            _netManager.PollEvents();
        }

        public void BroadcastGameState(GameState state)
        {
            if (!IsHost) return;

            NetDataWriter writer = new NetDataWriter();
            string json = JsonSerializer.Serialize(state);
            writer.Put((byte)PacketType.GameStateUpdate);
            writer.Put(json);

            _netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            if (reader.AvailableBytes == 0) return;

            byte packetId = reader.GetByte();
            PacketType type = (PacketType)packetId;

            switch (type)
            {
                case PacketType.GameStateUpdate:
                    if (IsHost) return; // Host ignores its own state loopback if any
                    string json = reader.GetString();
                    try
                    {
                        var state = JsonSerializer.Deserialize<GameState>(json);
                        if (state != null)
                        {
                            OnGameStateReceived?.Invoke(state);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"Error parsing GameState: {ex.Message}");
                    }
                    break;

                case PacketType.JoinRequest:
                    // Handle join request
                    break;
            }
            
            reader.Recycle();
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Log($"Peer connected: {peer.EndPoint}");
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
             Log($"Peer disconnected: {disconnectInfo.Reason}");
        }

        private void Log(string message)
        {
            OnLog?.Invoke($"[Net] {message}");
            Console.WriteLine($"[Net] {message}");
        }
    }
}
