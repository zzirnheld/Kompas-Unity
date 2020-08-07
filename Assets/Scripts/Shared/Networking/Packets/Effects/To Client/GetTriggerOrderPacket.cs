using KompasClient.Effects;
using KompasClient.GameCore;
using KompasCore.Networking;
using System.Linq;

namespace KompasCore.Networking
{
    public class GetTriggerOrderPacket : Packet
    {
        public int[] sourceCardIds;
        public int[] effIndices;

        public GetTriggerOrderPacket() : base(GetTriggerOrder) { }

        public GetTriggerOrderPacket(int[] sourceCardIds, int[] effIndices) : this()
        {
            this.sourceCardIds = sourceCardIds;
            this.effIndices = effIndices;
        }

        public override Packet Copy() => new GetTriggerOrderPacket(sourceCardIds, effIndices);
    }
}

namespace KompasClient.Networking
{
    public class GetTriggerOrderClientPacket : GetTriggerOrderPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var trigger = clientGame.GetCardWithID(sourceCardId)?.Effects.ElementAt(effIndex).Trigger as ClientTrigger;
            if (trigger == null) return;
            trigger.ClientEffect.ClientController = clientGame.ClientPlayers[0];
            clientGame.clientUICtrl.ShowOptionalTrigger(trigger, effIndex);
        }
    }
}