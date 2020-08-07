using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;

namespace KompasCore.Networking
{
    public class SelectXPacket : Packet
    {
        public int x;

        public SelectXPacket() : base(XSelectionChosen) { }

        public SelectXPacket(int x) : this()
        {
            this.x = x;
        }

        public override Packet Copy() => new SelectXPacket(x);
    }
}

namespace KompasServer.Networking
{
    public class SelectXServerPacket : SelectXPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            var currSubeff = serverGame.CurrEffect?.CurrSubeffect;

            if (currSubeff is PlayerChooseXSubeffect chooseXSubeffect)
                chooseXSubeffect.SetXIfLegal(x);
        }
    }
}