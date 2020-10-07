using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class DiscardController : MonoBehaviour
    {
        public Game game;
        public Player Owner;

        public readonly List<GameCard> Discard = new List<GameCard>();

        //adding/removing cards
        public virtual bool AddToDiscard(GameCard card, IStackable stackSrc = null)
        {
            Debug.Assert(card != null);
            Debug.Log("Adding to discard: " + card.CardName);
            card.Remove(stackSrc);
            Discard.Add(card);
            card.Controller = Owner;
            card.Location = CardLocation.Discard;
            return true;
        }

        public int IndexOf(GameCard card)
        {
            return Discard.IndexOf(card);
        }

        public virtual bool RemoveFromDiscard(GameCard card)
        {
            if (!Discard.Contains(card)) return false;

            Discard.Remove(card);
            return true;
        }

        public List<GameCard> CardsThatFitRestriction(CardRestriction cardRestriction)
        {
            List<GameCard> cards = new List<GameCard>();

            foreach (GameCard c in Discard)
            {
                if (cardRestriction.Evaluate(c)) cards.Add(c);
            }

            return cards;
        }

        public void SpreadOutCards()
        {
            int wrapLen = (int)(Mathf.Sqrt(Discard.Count) + 0.5f);
            int x = 0, y = 0;
            for(int i = 0; i < Discard.Count; i++)
            {
                Discard[i].transform.localPosition = new Vector3(x, 0, -1 * y);

                x = (x + 1) % wrapLen;
                if (x == 0) y++;
            }
        }
    }
}