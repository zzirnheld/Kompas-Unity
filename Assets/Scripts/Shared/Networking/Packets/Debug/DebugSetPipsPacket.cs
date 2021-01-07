using KompasCore.Networking;
using KompasServer.GameCore;

namespace KompasCore.Networking
{
    public class DebugSetPipsPacket : Packet
    {
        public int numPips;

        public DebugSetPipsPacket() : base(DebugSetPips) { }

        public DebugSetPipsPacket(int numPips) : this()
        {
            this.numPips = numPips;
        }

        public override Packet Copy() => new DebugSetPipsPacket(numPips);
    }
}

namespace KompasServer.Networking
{
    public class DebugSetPipsServerPacket : DebugSetPipsPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            if (serverGame.uiCtrl.DebugMode)
            {
                UnityEngine.Debug.LogWarning($"Debug setting player {player.index} pips to {numPips}");
                player.Pips = numPips;
            }
            else UnityEngine.Debug.LogError($"Tried to debug set pips of player {player.index} to {numPips} while NOT in debug mode!");
        }
    }
}