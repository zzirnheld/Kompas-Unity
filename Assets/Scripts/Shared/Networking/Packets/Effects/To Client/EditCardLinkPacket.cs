
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;
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

        public EditCardLinkPacket() : base(EditCardLink) { }

        public EditCardLinkPacket(int[] linkedCardsIDs, int effIndex, int whoseEffectID, bool add) : this()
        {
            this.linkedCardsIDs = linkedCardsIDs;
            this.effIndex = effIndex;
            this.whoseEffectID = whoseEffectID;
            this.add = add;
        }

        public EditCardLinkPacket(IEnumerable<GameCard> cards, Effect eff, bool add)
            : this(cards.Select(c => c.ID).ToArray(), eff.EffectIndex, eff.Source.ID, add)
        { }

        public EditCardLinkPacket(CardLink cardLink, bool add)
            : this(cardLink.Cards, cardLink.LinkingEffect, add)
        { }

        public override Packet Copy() => new EditCardLinkPacket(linkedCardsIDs, effIndex, whoseEffectID, add);
    }
}

namespace KompasClient.Networking
{
    public class EditCardLinkClientPacket : EditCardLinkPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var effect = clientGame.GetCardWithID(whoseEffectID)?.Effects.ElementAt(effIndex);
            var cards = linkedCardsIDs.Select(clientGame.GetCardWithID);

            if (effect == default || cards.Count() == 0) throw new System.ArgumentException($"Bad edit card args {linkedCardsIDs}, {effIndex}, {whoseEffectID}");
            var linkHandler = cards.First().CardLinkHandler;

            if (add) linkHandler.CreateLink(cards, effect);
            else linkHandler.RemoveEquivalentLink(cards, effect);
        }
    }
}