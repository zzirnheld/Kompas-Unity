using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class AttachCardPacket : Packet
    {
        public int cardId;
        public string cardName;
        public int controllerIndex;
        public int x;
        public int y;

        public AttachCardPacket() : base(AttachCard) { }

        public AttachCardPacket(int cardId, string cardName, int controllerIndex, int x, int y) : this()
        {
            this.cardId = cardId;
            this.cardName = cardName;
            this.controllerIndex = controllerIndex;
            this.x = controllerIndex == 0 ? x : 6 - x;
            this.y = controllerIndex == 0 ? y : 6 - y;
        }

        public override Packet Copy() => new AttachCardPacket(cardId, cardName, controllerIndex, x, y);

        public override Packet GetInversion(bool known)
        {
            if (known) return new AttachCardPacket(cardId, cardName, 1 - controllerIndex, x, y);
            else return new AddCardPacket(cardId, cardName, CardLocation.Field, controllerIndex, x, y, true);
        }
    }
}

namespace KompasClient.Networking
{
    public class AttachCardClientPacket : AttachCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var toAttach = clientGame.GetCardWithID(cardId);
            var attachedTo = clientGame.boardCtrl.GetCardAt(x, y);

            if (toAttach != null && attachedTo != null) attachedTo.AddAugment(toAttach);
        }
    }
}