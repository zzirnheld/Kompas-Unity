using KompasClient.GameCore;
using KompasCore.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class StackEmptyPacket : Packet
    {
        public StackEmptyPacket() : base(StackEmpty) { }

        public override Packet Copy() => new StackEmptyPacket();
    }
}

namespace KompasClient.Networking
{
    public class StackEmptyClientPacket : StackEmptyPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.StackEmptied();
    }
}