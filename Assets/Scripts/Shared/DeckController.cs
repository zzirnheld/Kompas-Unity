using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KompasCore.Helpers;

namespace KompasCore.GameCore
{
    public abstract class DeckController : OwnedGameLocation
    {
        public override CardLocation CardLocation => CardLocation.Deck;

        //rng for shuffling

        private readonly List<GameCard> deck = new List<GameCard>();
        public override IEnumerable<GameCard> Cards => deck;

        public override int IndexOf(GameCard card) => deck.IndexOf(card);
        public int DeckSize => deck.Count;
        public GameCard Topdeck => deck.FirstOrDefault();
        public GameCard Bottomdeck => deck.LastOrDefault();

        /// <summary>
        /// Sets the card's information to match this deck, but doesn't set its index.
        /// </summary>
        /// <param name="card">The card to add to this deck</param>
        /// <returns><see langword="true"/> if the add was completely successful.<br></br>
        /// <see langword="false"/> if the add failed in a way that isn't considered "impossible" (i.e. removing an avatar)</returns>
        protected virtual bool AddToDeck(GameCard card, int? index = null, IStackable stackSrc = null)
        {
            if (card == null) throw new NullCardException("Null card to add to deck");
            //Does not check if card is already in deck, because the functions to move around a card in deck are the same as those to add a card to deck
            //Check if the card is successfully removed (if it's not, it's probably an avatar)
            if (card.Remove(stackSrc))
            {
                if (index.HasValue) deck.Insert(index.Value, card);
                else deck.Add(card);
                card.GameLocation = this;
                card.Controller = Owner;
                card.Position = null;
                return true;
            }
            return false;
        }

        //adding and removing cards
        public virtual bool PushTopdeck(GameCard card, IStackable stackSrc = null)
            => AddToDeck(card, index: 0, stackSrc: stackSrc);

        public virtual bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
            => AddToDeck(card, stackSrc: stackSrc);

        public virtual bool ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            bool ret = AddToDeck(card, stackSrc: stackSrc);
            if (ret) Shuffle();
            return ret;
        }

        /// <summary>
        /// Random access remove from deck
        /// </summary>
        public override void Remove(GameCard card)
        {
            if (!deck.Contains(card))
                throw new CardNotHereException(CardLocation, card, $"Couldn't remove {card.CardName} from deck, it wasn't in deck!");

            deck.Remove(card);
        }

        //misc

        public void Shuffle() => CollectionsHelper.ShuffleInPlace(deck);

        public static void BottomdeckMany(IEnumerable<GameCard> cards, IStackable stackSrc = null)
        {
            var toShuffleInOrder = CollectionsHelper.Shuffle(cards.ToList());
            foreach (var card in toShuffleInOrder) card.Bottomdeck(stackSrc);
        }

        public List<GameCard> CardsThatFitRestriction(CardRestriction cardRestriction, ResolutionContext context)
        {
            List<GameCard> cards = new List<GameCard>();
            foreach (GameCard c in deck)
            {
                if (c != null && cardRestriction.IsValidCard(c, context))
                    cards.Add(c);
            }
            return cards;
        }
    }
}