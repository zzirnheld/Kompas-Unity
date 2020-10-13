using KompasClient.GameCore;
using KompasCore.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class TargetAcceptedPacket : Packet
    {
        public TargetAcceptedPacket() : base(TargetAccepted) { }

        public override Packet Copy() => new TargetAcceptedPacket();
    }
}

namespace KompasClient.Networking
{
    public class TargetAcceptedClientPacket : TargetAcceptedPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.targetMode = Game.TargetMode.Free;
            clientGame.CurrCardRestriction = null;
            clientGame.CurrSpaceRestriction = null;
            clientGame.clientUICtrl.SetCurrState("Target Accepted");
            clientGame.ShowNoTargets();
        }
    }
}