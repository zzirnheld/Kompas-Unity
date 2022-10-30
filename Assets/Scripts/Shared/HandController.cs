using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.GameCore
{
    public abstract class HandController : MonoBehaviour, IGameLocation
    {
        public abstract Player Owner { get; }

        public CardLocation CardLocation => CardLocation.Hand;

        protected readonly List<GameCard> hand = new List<GameCard>();

        public int HandSize => hand.Count;
        public int IndexOf(GameCard card) => hand.IndexOf(card);

        public virtual bool Hand(GameCard card, IStackable stackSrc = null)
        {
            if (card == null) throw new NullCardException("Cannot add null card to hand");

            var successful = card.Remove(stackSrc);
            if (successful)
            {
                Debug.Log($"Handing {card.CardName}");

                hand.Add(card);
                card.GameLocation = this;
                card.Position = null;
                card.Controller = Owner; //TODO should this be before or after the prev line?

                SpreadOutCards();
            }
            return successful;
        }

        public virtual void Remove(GameCard card)
        {
            if (!hand.Contains(card)) throw new CardNotHereException(CardLocation, card,
                $"Hand of \n{string.Join(", ", hand.Select(c => c.CardName))}\n doesn't contain {card}, can't remove it!");

            hand.Remove(card);
            SpreadOutCards();
        }

        public void SpreadOutCards()
        {
            //iterate through children, set the z coord
            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].CardController.transform.parent = transform;
                hand[i].CardController.transform.localPosition = new Vector3((-0.9f * (float)hand.Count) + ((float)i * 2.25f), 0, 0);
                hand[i].CardController.SetRotation();
                hand[i].CardController.gameObject.SetActive(true);
            }
        }
    }
}