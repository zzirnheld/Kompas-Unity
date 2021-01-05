using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
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
            if (card == null)
            {
                Debug.LogError("Cannot add null card to hand");
                return false;
            }

            if (!card.Remove(stackSrc))
            {
                Debug.LogWarning($"Could not remove card named {card.CardName} in location {card.Location}");
                return false;
            }

            hand.Add(card);
            card.Location = CardLocation.Hand;
            card.Controller = Owner;

            card.transform.rotation = Quaternion.Euler(90, 0, 0);
            SpreadOutCards();
            return true;
        }

        public virtual bool RemoveFromHand(GameCard card)
        {
            if (!hand.Contains(card))
            {
                Debug.LogError($"Hand of \n{string.Join(", ", hand.Select(c => c.CardName))}\n doesn't contain {card}, can't remove it!");
                return false;
            }

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