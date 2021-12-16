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
        public Player Owner;

        public CardLocation CardLocation => CardLocation.Hand;
        public readonly List<GameCard> hand = new List<GameCard>();

        public int HandSize => hand.Count;
        public int IndexOf(GameCard card) => hand.IndexOf(card);

        public virtual void Add(GameCard card, IStackable stackSrc = null)
        {
            if (card == null) throw new NullCardException("Cannot add null card to hand");

            card.Remove(stackSrc);

            hand.Add(card);
            card.GameLocation = this;
            card.Position = null;
            card.Controller = Owner; //TODO should this be before or after the prev line?

            card.transform.rotation = Quaternion.Euler(90, 0, 0);
            SpreadOutCards();
        }

        public virtual void Remove(GameCard card)
        {
            if (!hand.Contains(card)) throw new CardNotHereException(CardLocation, 
                $"Hand of \n{string.Join(", ", hand.Select(c => c.CardName))}\n doesn't contain {card}, can't remove it!");

            hand.Remove(card);
            SpreadOutCards();
        }

        public virtual void SpreadOutCards()
        {
            //iterate through children, set the z coord
            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].transform.localPosition = new Vector3((-0.8f * (float)hand.Count) + ((float)i * 2f), 0, 0);
                hand[i].transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }
}