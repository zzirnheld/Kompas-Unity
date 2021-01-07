using KompasCore.Networking;
using KompasServer.GameCore;

namespace KompasCore.Networking
{
    public class DebugDrawPacket : Packet
    {
        public DebugDrawPacket() : base(DebugDraw) { }

        public override Packet Copy() => new DebugDrawPacket();
    }
}

namespace KompasServer.Networking
{
    public class DebugDrawServerPacket : DebugDrawPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            if (serverGame.uiCtrl.DebugMode)
            {
                UnityEngine.Debug.LogWarning($"Debug drawing");
                serverGame.Draw(player.index);
            }
            else UnityEngine.Debug.LogError($"Tried to debug draw while NOT in debug mode!");
        }
    }
}