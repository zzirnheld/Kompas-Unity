﻿using KompasCore.Networking;
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

        public AddCardPacket() : base(AddCard) { }

        public AddCardPacket(int cardId, string cardName, CardLocation location, int controllerIndex, bool invert = false) : this()
        {
            this.cardId = cardId;
            this.cardName = cardName;
            this.location = location;
            this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
        }

        public AddCardPacket(int cardId, string cardName, CardLocation location, int controllerIndex, int x, int y, bool attached, bool invert = false) 
            : this(cardId, cardName, location, controllerIndex, invert)
        {
            this.x = invert ? 6 - x : x;
            this.y = invert ? 6 - y : y;
            this.attached = attached;
        }

        public override Packet Copy() => new AddCardPacket(cardId, cardName, location, controllerIndex, x, y, attached);

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
            else return new AddCardPacket(cardId, cardName, location, controllerIndex, x, y, attached, invert: true);
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
            var card = clientGame.cardRepo.InstantiateClientNonAvatar(cardName, clientGame, controller, cardId);
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