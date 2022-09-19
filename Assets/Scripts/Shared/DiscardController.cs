using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using KompasCore.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class DiscardController : AbstractGameLocation
    {
        public DiscardUIController discardUIController;

        protected readonly List<GameCard> discard = new List<GameCard>();

        public override CardLocation CardLocation => CardLocation.Discard;
        public IReadOnlyCollection<GameCard> Cards => discard;

        //adding/removing cards
        public virtual bool Discard(GameCard card, IStackable stackSrc = null)
        {
            if (card == null) throw new NullCardException("Cannot add null card to hand");

            //Check if the card is successfully removed (if it's not, it's probably an avatar)
            bool successful = card.Remove(stackSrc);
            if (successful)
            {
                Debug.Log($"Discarding {card.CardName}");
                discard.Add(card);
                card.Controller = owner;
                card.GameLocation = this;
                card.Position = null;
                discardUIController.Refresh();
            }
            return successful;
        }

        public override void Remove(GameCard card)
        {
            if (!discard.Contains(card)) throw new CardNotHereException(CardLocation.Discard, card);

            discard.Remove(card);
            discardUIController.Refresh();
        }

        public override int IndexOf(GameCard card) => discard.IndexOf(card);

        public List<GameCard> CardsThatFit(Func<GameCardBase, bool> cardRestriction)
        {
            List<GameCard> cards = new List<GameCard>();

            foreach (GameCard c in discard)
            {
                if (cardRestriction(c)) cards.Add(c);
            }

            return cards;
        }
    }
}