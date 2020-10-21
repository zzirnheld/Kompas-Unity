using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetCardTargetPacket : Packet
    {
        public string sourceCardName;
        public string targetBlurb;
        public int[] potentialTargetIds;
        public int num;

        public GetCardTargetPacket() : base(GetCardTarget) { }

        public GetCardTargetPacket(string sourceCardName, string targetBlurb, int[] potentialTargetIds, int num = 1) : this()
        {
            this.sourceCardName = sourceCardName;
            this.targetBlurb = targetBlurb;
            this.potentialTargetIds = potentialTargetIds;
            this.num = num;
        }

        public override Packet Copy() => new GetCardTargetPacket(sourceCardName, targetBlurb, potentialTargetIds);
    }
}

namespace KompasClient.Networking
{
    public class GetCardTargetClientPacket : GetCardTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.targetMode = Game.TargetMode.CardTarget;
            clientGame.PotentialTargetIds = potentialTargetIds;
            clientGame.clientUICtrl.StartSearch(clientGame.CurrentPotentialTargets, numToChoose: num);
            clientGame.clientUICtrl.SetCurrState($"Choose {sourceCardName}'s Card Target", targetBlurb);
            clientGame.ShowValidCardTargets();
        }
    }
}