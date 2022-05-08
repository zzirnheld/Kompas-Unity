using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.GameCore
{
    public abstract class DeckController : MonoBehaviour, IGameLocation
    {
        public const string BLANK_CARD_PATH = "Card Jsons/Blank Card";

        public Game game;
        public abstract Player Owner { get; }

        public CardLocation CardLocation => CardLocation.Deck;

        //rng for shuffling
        private static readonly System.Random rng = new System.Random();

        public readonly List<GameCard> Deck = new List<GameCard>();

        public int IndexOf(GameCard card) => Deck.IndexOf(card);
        public int DeckSize => Deck.Count;
        public GameCard Topdeck => Deck.FirstOrDefault();
        public GameCard Bottomdeck => Deck.LastOrDefault();

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
            if (ret) Deck.Insert(0, card);
            return ret;
        }

        public virtual bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            bool ret = AddToDeck(card, stackSrc);
            if (ret) Deck.Add(card);
            return ret;
        }

        public virtual bool ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            bool ret = AddToDeck(card, stackSrc);
            if (ret)
            {
                Deck.Add(card);
                Shuffle();
            }
            return ret;
        }

        /// <summary>
        /// Random access remove from deck
        /// </summary>
        public virtual void Remove(GameCard card)
        {
            if (!Deck.Contains(card))
                throw new CardNotHereException(CardLocation, card, $"Couldn't remove {card.CardName} from deck, it wasn't in deck!");

            Deck.Remove(card);
        }

        //misc
        public void Shuffle()
        {
            int n = Deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                GameCard value = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = value;
            }
        }

        public List<GameCard> CardsThatFitRestriction(CardRestriction cardRestriction, ActivationContext context)
        {
            List<GameCard> cards = new List<GameCard>();
            foreach (GameCard c in Deck)
            {
                if (c != null && cardRestriction.IsValidCard(c, context))
                    cards.Add(c);
            }
            return cards;
        }
    }
}