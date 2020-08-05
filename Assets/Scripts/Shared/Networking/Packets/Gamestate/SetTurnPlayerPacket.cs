using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class SetTurnPlayerPacket : Packet
    {
        public int turnPlayer;

        public SetTurnPlayerPacket() : base(SetTurnPlayer) { }

        public SetTurnPlayerPacket(int turnPlayer, bool invert = false) : this()
        {
            this.turnPlayer = invert ? 1 - turnPlayer : turnPlayer;
        }

        public override Packet Copy() => new SetTurnPlayerPacket(turnPlayer);

        public override Packet GetInversion(bool known) => new SetTurnPlayerPacket(turnPlayer, invert: true);
    }
}

namespace KompasClient.Networking
{
    public class SetTurnPlayerClientPacket : SetTurnPlayerPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.EndTurn();
    }
}