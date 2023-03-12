using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class DeckAcceptedPacket : Packet
    {
        public DeckAcceptedPacket() : base(DeckAccepted) { }

        public override Packet Copy() => new DeckAcceptedPacket();
    }
}

namespace KompasClient.Networking
{
    public class DeckAcceptedClientPacket : DeckAcceptedPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.clientUIController.ShowDeckAcceptedUI();
    }
}