using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class PutCardsBackPacket : Packet
    {
        public PutCardsBackPacket() : base(PutCardsBack) { }

        public override Packet Copy() => new PutCardsBackPacket();
    }
}

namespace KompasClient.Networking
{
    public class PutCardsBackClientPacket : PutCardsBackPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.PutCardsBack();
    }
}