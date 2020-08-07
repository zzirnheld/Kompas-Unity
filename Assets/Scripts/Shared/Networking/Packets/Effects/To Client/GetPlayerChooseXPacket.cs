using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class GetPlayerChooseXPacket : Packet
    {
        public GetPlayerChooseXPacket() : base(PlayerChooseX) { }

        public override Packet Copy() => new GetPlayerChooseXPacket();
    }
}

namespace KompasClient.Networking
{
    public class GetPlayerChooseXClientPacket : GetPlayerChooseXPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.clientUICtrl.GetXForEffect();
        }
    }
}