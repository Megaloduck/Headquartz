namespace Headquartz.Services.Networking
{
    public enum PacketType : byte
    {
        JoinRequest = 1,
        GameStateUpdate = 2,
        PlayerAction = 3
    }
}
