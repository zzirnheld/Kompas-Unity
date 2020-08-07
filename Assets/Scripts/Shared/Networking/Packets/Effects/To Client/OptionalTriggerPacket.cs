using KompasCore.Networking;
using KompasClient.GameCore;
using KompasServer.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class OptionalTriggerPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;

        public OptionalTriggerPacket() : base(OptionalTrigger) { }

        public OptionalTriggerPacket(int sourceCardId, int effIndex) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
        }

        public override Packet Copy() => new OptionalTriggerPacket(sourceCardId, effIndex);
    }
}

namespace KompasClient.Networking
{
    public class OptionalTriggerClientPacket : OptionalTriggerPacket, IClientOrderPacket
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