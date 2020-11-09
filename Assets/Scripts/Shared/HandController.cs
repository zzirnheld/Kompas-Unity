using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    public abstract class HandController : MonoBehaviour
    {
        public Player Owner;

        public readonly List<GameCard> hand = new List<GameCard>();

        public int HandSize => hand.Count;
        public int IndexOf(GameCard card) => hand.IndexOf(card);

        public virtual bool AddToHand(GameCard card, IStackable stackSrc = null)
        {
            if (card == null) return false;
            card.Remove(stackSrc);
            hand.Add(card);
            card.ResetCard();
            card.Location = CardLocation.Hand;
            card.Controller = Owner;

            card.transform.rotation = Quaternion.Euler(90, 0, 0);
            SpreadOutCards();
            return true;
        }

        public virtual bool RemoveFromHand(GameCard card)
        {
            if (!hand.Contains(card)) return false;

            hand.Remove(card);
            SpreadOutCards();
            return true;
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