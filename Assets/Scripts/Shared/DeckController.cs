using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class DeckController : MonoBehaviour
    {
        public const string BLANK_CARD_PATH = "Card Jsons/Blank Card";

        public Game game;

        //one of these for each player
        public Player Owner;

        //rng for shuffling
        private static readonly System.Random rng = new System.Random();

        public List<GameCard> Deck { get; } = new List<GameCard>();

        public int IndexOf(GameCard card) => Deck.IndexOf(card);
        public int DeckSize => Deck.Count;

        /// <summary>
        /// Gets the card at the designated index.
        /// </summary>
        /// <param name="index">Index of the card to get</param>
        /// <param name="remove">Whether or not to remove the card</param>
        /// <param name="shuffle">Whether or not to shuffle the deck after getting the card</param>
        /// <returns></returns>
        public GameCard CardAt(int index, bool remove, bool shuffle = false)
        {
            if (index > Deck.Count()) return null;
            GameCard card = Deck.ElementAt(index);
            if (remove) Deck.RemoveAt(index);
            if (shuffle) Shuffle();
            return card;
        }

        protected virtual bool AddCard(GameCard card, IStackable stackSrc = null)
        {
            card.Remove(stackSrc);
            card.Location = CardLocation.Deck;
            card.Controller = Owner;
            return true;
        }

        //adding and removing cards
        public virtual bool PushTopdeck(GameCard card, IStackable stackSrc = null)
        {
            AddCard(card, stackSrc);
            Deck.Insert(0, card);
            return true;
        }

        public virtual bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            AddCard(card, stackSrc);
            Deck.Add(card);
            return true;
        }

        public virtual bool ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            AddCard(card, stackSrc);
            Deck.Add(card);
            Shuffle();
            return true;
        }

        public GameCard PopTopdeck()
        {
            if (Deck.Count == 0) return null;

            GameCard card = Deck[0];
            Deck.RemoveAt(0);
            return card;
        }

        public GameCard PopBottomdeck()
        {
            if (Deck.Count == 0) return null;

            GameCard card = Deck[Deck.Count - 1];
            Deck.RemoveAt(Deck.Count - 1);
            return card;
        }

        public GameCard RemoveCardWithName(string name)
        {
            GameCard toReturn;
            for (int i = 0; i < Deck.Count; i++)
            {
                if (Deck[i].CardName.Equals(name))
                {
                    toReturn = Deck[i];
                    Deck.RemoveAt(i);
                    return toReturn;
                }
            }
            return null;
        }

        /// <summary>
        /// Random access remove from deck
        /// </summary>
        public void RemoveFromDeck(GameCard card)
        {
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

        /// <summary>
        /// Checks if a card exists that fits the given restriction
        /// </summary>
        /// <param name="cardRestriction"></param>
        /// <returns></returns>
        public bool Exists(CardRestriction cardRestriction)
        {
            foreach (GameCard c in Deck)
            {
                if (c != null && cardRestriction.Evaluate(c)) return true;
            }

            return false;
        }

        public List<GameCard> CardsThatFitRestriction(CardRestriction cardRestriction)
        {
            List<GameCard> cards = new List<GameCard>();
            foreach (GameCard c in Deck)
            {
                if (c != null && cardRestriction.Evaluate(c))
                    cards.Add(c);
            }
            return cards;
        }
    }
}