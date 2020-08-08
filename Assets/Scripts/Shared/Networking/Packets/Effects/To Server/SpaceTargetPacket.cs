using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;

namespace KompasCore.Networking
{
    public class SpaceTargetPacket : Packet
    {
        public int x;
        public int y;

        public SpaceTargetPacket() : base(SpaceTargetChosen) { }

        public SpaceTargetPacket(int x, int y) : this()
        {
            this.x = x;
            this.y = y;
        }

        public override Packet Copy() => new SpaceTargetPacket(x, y);
    }
}

namespace KompasServer.Networking
{
    public class SpaceTargetServerPacket : SpaceTargetPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            var currSubeff = serverGame.CurrEffect?.CurrSubeffect;
            if(player.index != 0)
            {
                x = 6 - x;
                y = 6 - y;
            }

            if (currSubeff is SpaceTargetSubeffect spaceTargetSubeffect)
                spaceTargetSubeffect.SetTargetIfValid(x, y);
        }
    }
}