using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class GetDeckPacket : Packet {
        public GetDeckPacket() : base(GetDeck) { }

        public override Packet Copy() => new GetDeckPacket();
    }
}

namespace KompasClient.Networking
{
    public class GetDeckClientPacket : GetDeckPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.clientUICtrl.ShowGetDecklistUI();
    }
}