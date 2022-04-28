using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    //Not abstract because Client uses this base class
    public class AnnihilationController : MonoBehaviour, IGameLocation
    {
        public Game game;
        public Player owner;

        public CardLocation CardLocation => CardLocation.Annihilation;
        public List<GameCard> Cards { get; } = new List<GameCard>();


        /// <summary>
        /// Annihilates the card
        /// </summary>
        /// <param name="card">The card to add to this game location</param>
        /// <returns><see langword="true"/> if the add was completely successful.<br></br>
        /// <see langword="false"/> if the add failed in a way that isn't considered "impossible" (i.e. removing an avatar)</returns>
        public virtual bool Annihilate(GameCard card, IStackable stackSrc = null)
        {
            if (card == null) throw new NullCardException("Cannot add null card to hand");
            Debug.Log($"Annihilating {card.CardName} from {card.Location}");

            if (card.GameLocation != null && card.GameLocation.Equals(this))
                throw new AlreadyHereException(CardLocation.Annihilation);

            //Check if the card is successfully removed (if it's not, it's probably an avatar)
            if (card.Remove(stackSrc))
            {
                Cards.Add(card);
                card.GameLocation = this;
                card.Position = null;
                SpreadOutCards();
                return true;
            }
            return false;
        }

        public virtual void Remove(GameCard card)
        {
            if (!Cards.Contains(card))
                throw new CardNotHereException(CardLocation.Annihilation, card, "Card was not in annihilation, couldn't be removed");

            Cards.Remove(card);
            SpreadOutCards();
        }

        public int IndexOf(GameCard card) => Cards.IndexOf(card);

        public void SpreadOutCards()
        {
            float spreadOutMultipler = 2f * (owner.index == 0 ? -1f : 1f);
            int max = Cards.Count - 1;

            //iterate through children, set the z coord
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.localPosition = new Vector3(spreadOutMultipler * (float)(max - i), 0, 0);
            }
        }
    }
}