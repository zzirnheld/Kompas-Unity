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
        public int x;
        public bool showX;

        public OptionalTriggerPacket() : base(OptionalTrigger) { }

        public OptionalTriggerPacket(int sourceCardId, int effIndex, int x, bool showX) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.x = x;
            this.showX = showX;
        }

        public override Packet Copy() => new OptionalTriggerPacket(sourceCardId, effIndex, x, showX);
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
            clientGame.clientUICtrl.ShowOptionalTrigger(trigger, showX, x);
        }
    }
}