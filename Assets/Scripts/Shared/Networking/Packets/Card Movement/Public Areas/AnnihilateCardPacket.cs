using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class AnnihilateCardPacket : Packet
    {
        public int cardId;
        public string cardName;
        public int controllerIndex;

        public AnnihilateCardPacket() : base(DiscardCard) { }

        public AnnihilateCardPacket(int cardId, string cardName, int controllerIndex, bool invert = false) : this()
        {
            this.cardId = cardId;
            this.cardName = cardName;
            this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
        }

        public override Packet Copy() => new AnnihilateCardPacket(cardId, cardName, controllerIndex, invert: false);

        public override Packet GetInversion(bool known)
        {
            if (known) return new AnnihilateCardPacket(cardId, cardName, controllerIndex, invert: true);
            else return new AddCardPacket(cardId, cardName, CardLocation.Annihilation, controllerIndex, invert: true);
        }
    }
}

namespace KompasClient.Networking
{
    public class AnnihilateCardClientPacket : AnnihilateCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            if (card != null) clientGame.annihilationCtrl.Annihilate(card);
        }
    }
}