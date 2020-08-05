using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;
using KompasCore.GameCore;

namespace KompasCore.Networking
{
    public class AddCardPacket : Packet
    {
        public int cardId;
        public string cardName;
        public CardLocation location;
        public int controllerIndex;
        public int x;
        public int y;
        public bool attached;

        public AddCardPacket() : base(DeleteCard) { }

        public AddCardPacket(int cardId, string cardName, CardLocation location, int controllerIndex) : this()
        {
            this.cardId = cardId;
            this.cardName = cardName;
            this.controllerIndex = controllerIndex;
        }

        public AddCardPacket(int cardId, string cardName, CardLocation location, int controllerIndex, int x, int y, bool attached) 
            : this(cardId, cardName, location, controllerIndex)
        {
            if(controllerIndex == 0)
            {
                this.x = x;
                this.y = y;
            }
            else
            {
                this.x = 6 - x;
                this.y = 6 - y;
            }

            this.attached = attached;
        }

        public override Packet Copy()
        {
            var p = new AddCardPacket(cardId, cardName, location, controllerIndex)
            {
                x = x,
                y = y
            };
            return p;
        }

        public override Packet GetInversion(bool known)
        {
            if (Game.HiddenLocations.Contains(location))
            {
                switch (location)
                {
                    case CardLocation.Hand: return new ChangeEnemyHandCountPacket(1);
                    case CardLocation.Deck: return null;
                    default: throw new System.ArgumentException($"What should add card packet do when a card is added to the hidden location {location}");
                }
            }
            else return new AddCardPacket(cardId, cardName, location, 1 - controllerIndex, x, y, attached);
        }
    }
}

namespace KompasClient.Networking
{
    public class AddCardClientPacket : AddCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var controller = clientGame.ClientPlayers[controllerIndex];
            var card = clientGame.cardRepo.InstantiateClientAvatar(cardName, clientGame, controller, cardId);
            clientGame.cardsByID.Add(cardId, card);
            switch (location)
            {
                case CardLocation.Nowhere: break;
                case CardLocation.Field:
                    if (attached) clientGame.boardCtrl.GetCardAt(x, y).AddAugment(card);
                    else card.Play(x, y, controller);
                    break;
                case CardLocation.Discard:
                    card.Discard();
                    break;
                case CardLocation.Hand:
                    card.Rehand();
                    break;
                case CardLocation.Deck:
                    card.Topdeck();
                    break;
                case CardLocation.Annihilation:
                    clientGame.annihilationCtrl.Annihilate(card);
                    break;
                default:
                    throw new System.ArgumentException($"Invalid location {location} for Add Card Client Packet to put card");
            }
        }
    }
}