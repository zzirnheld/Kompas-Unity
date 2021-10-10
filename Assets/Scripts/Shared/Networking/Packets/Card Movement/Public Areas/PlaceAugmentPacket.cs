using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Cards;

namespace KompasCore.Networking
{
    public class PlaceAugmentPacket : Packet
    {
        public int cardId;
        public string json;
        public int controllerIndex;
        public int x;
        public int y;

        public PlaceAugmentPacket() : base(AttachCard) { }

        public PlaceAugmentPacket(int cardId, string json, int controllerIndex, int x, int y, bool invert = false) : this()
        {
            this.cardId = cardId;
            this.json = json;
            this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
            this.x = invert ? 6 - x : x;
            this.y = invert ? 6 - y : y;
        }

        public PlaceAugmentPacket(GameCard card, int x, int y, bool invert = false)
            : this(card.ID, card.BaseJson, card.ControllerIndex, x, y, invert: invert)
        { }

        public override Packet Copy() => new PlaceAugmentPacket(cardId, json, controllerIndex, x, y);

        public override Packet GetInversion(bool known)
        {
            if (known) return new PlaceAugmentPacket(cardId, json, controllerIndex, x, y, invert: true);
            else return new AddCardPacket(cardId, json, CardLocation.Field, controllerIndex, x, y, 
                attached: false, placedAsAug: true, known: true, invert: true);
        }
    }
}

namespace KompasClient.Networking
{
    public class PlaceAugmentClientPacket : PlaceAugmentPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var toAttach = clientGame.GetCardWithID(cardId);
            if (toAttach != null)  clientGame.boardCtrl.PlaceAugment(toAttach, (x, y));
        }
    }
}