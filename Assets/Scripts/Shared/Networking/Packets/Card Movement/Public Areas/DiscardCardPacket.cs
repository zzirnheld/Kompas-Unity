using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Cards;

namespace KompasCore.Networking
{
    public class DiscardCardPacket : Packet
    {
        public int cardId;
        public string json;
        public int controllerIndex;

        public DiscardCardPacket() : base(DiscardCard) { }

        public DiscardCardPacket(int cardId, string json, int controllerIndex, bool invert = false) : this()
        {
            this.cardId = cardId;
            this.json = json;
            this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
        }

        public DiscardCardPacket(GameCard card, bool invert = false)
            : this(card.ID, card.BaseJson, card.ControllerIndex, invert: invert)
        { }

        public override Packet Copy() => new DiscardCardPacket(cardId, json, controllerIndex, invert: false);

        public override Packet GetInversion(bool known)
        {
            if (known) return new DiscardCardPacket(cardId, json, controllerIndex, invert: true);
            else return new AddCardPacket(cardId, json, CardLocation.Discard, controllerIndex, invert: true);
        }
    }
}

namespace KompasClient.Networking
{
    public class DiscardCardClientPacket : DiscardCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            if (card != null) card.Discard();
        }
    }
}