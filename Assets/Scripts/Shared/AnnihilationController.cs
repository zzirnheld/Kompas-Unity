using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class AnnihilationController : MonoBehaviour
    {
        public Game game;
        public Player owner;

        public List<GameCard> Cards { get; } = new List<GameCard>();

        public virtual bool Annihilate(GameCard card, IStackable stackSrc = null)
        {
            if(!card.Remove(stackSrc)) return false;
            Cards.Add(card);
            card.Location = CardLocation.Annihilation;
            SpreadOutCards();
            return true;
        }

        public virtual bool Remove(GameCard card)
        {
            if (!Cards.Contains(card)) return false;

            Cards.Remove(card);
            SpreadOutCards();
            return true;
        }

        public void SpreadOutCards()
        {
            float spreadOutMultipler = 2f * (owner.Index == 0 ? -1f : 1f);
            int max = Cards.Count - 1;

            //iterate through children, set the z coord
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.localPosition = new Vector3(spreadOutMultipler * (float)(max - i), 0, 0);
            }
        }
    }
}