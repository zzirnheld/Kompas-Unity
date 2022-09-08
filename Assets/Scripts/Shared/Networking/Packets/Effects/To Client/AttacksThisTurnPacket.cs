using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class AttacksThisTurnPacket : Packet
    {
        public int attackerId;
        public int attacksThisTurn;

        public AttacksThisTurnPacket() : base(AttacksThisTurn) { }

        public AttacksThisTurnPacket(int attackerId, int attacksThisTurn) : this()
        {
            this.attackerId = attackerId;
            this.attacksThisTurn = attacksThisTurn;
        }

        public override Packet Copy() => new AttacksThisTurnPacket(attackerId, attacksThisTurn);
    }
}

namespace KompasClient.Networking
{
    public class AttacksThisTurnClientPacket : AttacksThisTurnPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(attackerId);
            if (card != null)
            {
                card.SetAttacksThisTurn(attacksThisTurn);
                clientGame.UIController.cardViewController.Refresh();
            }
        }
    }
}