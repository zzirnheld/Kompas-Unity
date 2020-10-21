using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetSpaceTargetPacket : Packet
    {
        public string cardName;
        public string targetBlurb;
        public (int, int)[] potentialSpaces;

        public GetSpaceTargetPacket() : base(GetSpaceTarget) { }

        public GetSpaceTargetPacket(string cardName, string targetBlurb, (int, int)[] potentialSpaces) : this()
        {
            this.cardName = cardName;
            this.targetBlurb = targetBlurb;
            this.potentialSpaces = potentialSpaces;
        }

        public override Packet Copy() => new GetSpaceTargetPacket(cardName, targetBlurb, potentialSpaces);
    }
}

namespace KompasClient.Networking
{
    public class GetSpaceTargetClientPacket : GetSpaceTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.targetMode = Game.TargetMode.SpaceTarget;
            clientGame.CurrentPotentialSpaces = potentialSpaces;
            clientGame.clientUICtrl.SetCurrState($"Choose {cardName}'s Space Target", targetBlurb);
        }
    }
}