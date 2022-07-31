
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Networking;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Networking
{
    public class EditCardLinkPacket : Packet
    {
        public int[] linkedCardsIDs;

        public int effIndex;
        public int whoseEffectID;

        public bool add; //true for add, false for remove

        public EditCardLinkPacket() : base(AddTarget) { }

        public EditCardLinkPacket(int[] linkedCardsIDs, int effIndex, int whoseEffectID) : this()
        {
            this.linkedCardsIDs = linkedCardsIDs;
            this.effIndex = effIndex;
            this.whoseEffectID = whoseEffectID;
        }

        public override Packet Copy() => new EditCardLinkPacket(linkedCardsIDs, effIndex, whoseEffectID);
    }
}

namespace KompasClient.Networking
{
    public class EditCardLinkClientPacket : EditCardLinkPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var effect = clientGame.GetCardWithID(whoseEffectID)?.Effects.ElementAt(effIndex);
            var cards = new HashSet<GameCard>(linkedCardsIDs.Select(clientGame.GetCardWithID));

            if (effect == default || cards.Count == 0) throw new System.ArgumentException($"Bad edit card args {linkedCardsIDs}, {effIndex}, {whoseEffectID}");

            cards.First().CardLinkHandler.CreateLink(cards, effect);
        }
    }
}