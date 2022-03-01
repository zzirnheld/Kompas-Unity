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

        protected virtual void AddCard(GameCard card, IStackable stackSrc = null)
        {
            Debug.Log($"Adding {card.CardName} to deck from {card.Location}");
            card.Remove(stackSrc);
            card.GameLocation = this;
            card.Controller = Owner;
            card.Position = null;
        }

        //adding and removing cards
        public virtual void PushTopdeck(GameCard card, IStackable stackSrc = null)
        {
            AddCard(card, stackSrc);
            Deck.Insert(0, card);
        }

        public virtual void PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            AddCard(card, stackSrc);
            Deck.Add(card);
        }

        public virtual void ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            AddCard(card, stackSrc);
            Deck.Add(card);
            Shuffle();
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