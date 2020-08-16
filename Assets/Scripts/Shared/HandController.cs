using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    public abstract class HandController : MonoBehaviour
    {
        public Player Owner;

        public readonly List<GameCard> Hand = new List<GameCard>();

        public int HandSize => Hand.Count;

        public virtual bool AddToHand(GameCard card, IStackable stackSrc = null)
        {
            if (card == null) return false;
            card.Remove(stackSrc);
            Hand.Add(card);
            card.ResetCard();
            card.Location = CardLocation.Hand;
            card.Controller = Owner;

            card.transform.rotation = Quaternion.Euler(90, 0, 0);
            SpreadOutCards();
            return true;
        }

        public int IndexOf(GameCard card)
        {
            return Hand.IndexOf(card);
        }

        public virtual void RemoveFromHand(GameCard card)
        {
            Hand.Remove(card);
            SpreadOutCards();
        }

        public virtual void SpreadOutCards()
        {
            //iterate through children, set the z coord
            for (int i = 0; i < Hand.Count; i++)
            {
                Hand[i].transform.localPosition = new Vector3((-0.8f * (float)Hand.Count) + ((float)i * 2f), 0, 0);
                Hand[i].transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }
}