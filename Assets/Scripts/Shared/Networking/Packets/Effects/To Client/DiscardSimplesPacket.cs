using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class DiscardSimplesPacket : Packet
    {
        public DiscardSimplesPacket() : base(DiscardSimples) { }

        public override Packet Copy() => new DiscardSimplesPacket();
    }
}

namespace KompasClient.Networking
{
    public class DiscardSimplesClientPacket : DiscardSimplesPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.BoardController.ClearSpells();
    }
}