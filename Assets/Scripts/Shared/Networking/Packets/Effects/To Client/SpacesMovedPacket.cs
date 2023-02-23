using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class SpacesMovedPacket : Packet
    {
        public int cardId;
        public int spacesMoved;

        public SpacesMovedPacket() : base(SpacesMoved) { }

        public SpacesMovedPacket(int cardId, int spacesMoved) : this()
        {
            this.cardId = cardId;
            this.spacesMoved = spacesMoved;
        }

        public override Packet Copy() => new SpacesMovedPacket(cardId, spacesMoved);
    }
}

namespace KompasClient.Networking
{
    public class SpacesMovedClientPacket : SpacesMovedPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            if (card != null)
            {
                card.SpacesMoved = spacesMoved;
                clientGame.UIController.CardViewController.Refresh();
            }
        }
    }
}