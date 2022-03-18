using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class DiscardController : MonoBehaviour, IGameLocation
    {
        public Game game;
        public Player Owner;

        public CardLocation CardLocation => CardLocation.Discard;

        public readonly List<GameCard> Discard = new List<GameCard>();

        //adding/removing cards
        public virtual void Add(GameCard card, IStackable stackSrc = null)
        {
            if (card == null) throw new NullCardException("Cannot add null card to hand");
            Debug.Log($"Discarding {card.CardName} from {card.Location}");

            card.Remove(stackSrc);
            Discard.Add(card);
            card.Controller = Owner;
            card.GameLocation = this;
            card.Position = null;
        }

        public virtual void Remove(GameCard card)
        {
            if (!Discard.Contains(card)) throw new CardNotHereException(CardLocation.Discard, card);

            Discard.Remove(card);
            SpreadOutCards();
        }

        public int IndexOf(GameCard card)
        {
            return Discard.IndexOf(card);
        }

        public List<GameCard> CardsThatFit(Func<GameCardBase, bool> cardRestriction)
        {
            List<GameCard> cards = new List<GameCard>();

            foreach (GameCard c in Discard)
            {
                if (cardRestriction(c)) cards.Add(c);
            }

            return cards;
        }

        public virtual void SpreadOutCards()
        {
            int wrapLen = (int)(Mathf.Sqrt(Discard.Count) + 0.5f);
            int x = 0, y = 0;
            for (int i = 0; i < Discard.Count; i++)
            {
                Discard[i].transform.localPosition = new Vector3(2f * x, 0f, -2f * y);

                x = (x + 1) % wrapLen;
                if (x == 0) y++;
            }
        }
    }
}