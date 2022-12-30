﻿using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KompasCore.Helpers;

namespace KompasCore.GameCore
{
    public abstract class DeckController : MonoBehaviour, IGameLocation
    {
        public abstract Player Owner { get; }

        public CardLocation CardLocation => CardLocation.Deck;

        //rng for shuffling

        private readonly List<GameCard> deck = new List<GameCard>();
        public IReadOnlyCollection<GameCard> Deck => deck;

        public int IndexOf(GameCard card) => deck.IndexOf(card);
        public int DeckSize => deck.Count;
        public GameCard Topdeck => deck.FirstOrDefault();
        public GameCard Bottomdeck => deck.LastOrDefault();

        /// <summary>
        /// Sets the card's information to match this deck, but doesn't set its index.
        /// </summary>
        /// <param name="card">The card to add to this deck</param>
        /// <returns><see langword="true"/> if the add was completely successful.<br></br>
        /// <see langword="false"/> if the add failed in a way that isn't considered "impossible" (i.e. removing an avatar)</returns>
        protected virtual bool AddToDeck(GameCard card, IStackable stackSrc = null)
        {
            //Check if the card is successfully removed (if it's not, it's probably an avatar)
            if (card.Remove(stackSrc))
            {
                Debug.Log($"Adding {card.CardName} to deck from {card.Location}");
                card.GameLocation = this;
                card.Controller = Owner;
                card.Position = null;
                return true;
            }
            return false;
        }

        //adding and removing cards
        public virtual bool PushTopdeck(GameCard card, IStackable stackSrc = null)
        {
            bool ret = AddToDeck(card, stackSrc);
            if (ret) deck.Insert(0, card);
            return ret;
        }

        public virtual bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            bool ret = AddToDeck(card, stackSrc);
            if (ret) deck.Add(card);
            return ret;
        }

        public virtual bool ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            bool ret = AddToDeck(card, stackSrc);
            if (ret)
            {
                deck.Add(card);
                Shuffle();
            }
            return ret;
        }

        /// <summary>
        /// Random access remove from deck
        /// </summary>
        public virtual void Remove(GameCard card)
        {
            if (!deck.Contains(card))
                throw new CardNotHereException(CardLocation, card, $"Couldn't remove {card.CardName} from deck, it wasn't in deck!");

            deck.Remove(card);
        }

        //misc

        public void Shuffle() => CollectionsHelper.Shuffle(deck);

        public static void BottomdeckMany(IEnumerable<GameCard> cards, IStackable stackSrc = null)
        {
            var toShuffleInOrder = CollectionsHelper.Shuffle(cards.ToList());
            foreach (var card in toShuffleInOrder) card.Bottomdeck(stackSrc);
        }

        public List<GameCard> CardsThatFitRestriction(CardRestriction cardRestriction, ActivationContext context)
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