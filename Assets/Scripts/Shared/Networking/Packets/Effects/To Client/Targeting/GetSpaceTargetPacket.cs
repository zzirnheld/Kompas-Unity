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
        public int[] potentialSpaces;

        public GetSpaceTargetPacket() : base(GetSpaceTarget) { }

        public GetSpaceTargetPacket(string cardName, string targetBlurb, (int x, int y)[] potentialSpaces) : this()
        {
            this.cardName = cardName;
            this.targetBlurb = targetBlurb;
            this.potentialSpaces = potentialSpaces.Select(s => s.x * 7 + s.y).ToArray();
        }
    }
}

namespace KompasClient.Networking
{
    public class GetSpaceTargetClientPacket : GetSpaceTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.targetMode = Game.TargetMode.SpaceTarget;
            clientGame.CurrentPotentialSpaces = potentialSpaces.Select(s => (s / 7, s % 7)).ToArray();
            clientGame.clientUICtrl.SetCurrState($"Choose {cardName}'s Space Target", targetBlurb);
        }
    }
}