using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using KompasCore.UI;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    //Not abstract because Client uses this base class
    public abstract class AnnihilationController : OwnedGameLocation
    {
        public AnnihilationUIController annihilationUIController;

        private readonly List<GameCard> cards = new List<GameCard>();

        public override CardLocation CardLocation => CardLocation.Annihilation;
        public override IEnumerable<GameCard> Cards => cards;

        public override void Refresh() => annihilationUIController.Refresh();


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

            if (Equals(card.GameLocation)) throw new AlreadyHereException(CardLocation.Annihilation);

            //Check if the card is successfully removed (if it's not, it's probably an avatar)
            if (card.Remove(stackSrc))
            {
                cards.Add(card);
                card.GameLocation = this;
                card.Position = null;
                annihilationUIController.Refresh();
                return true;
            }
            return false;
        }

        public override void Remove(GameCard card)
        {
            if (!cards.Contains(card))
                throw new CardNotHereException(CardLocation.Annihilation, card, "Card was not in annihilation, couldn't be removed");

            cards.Remove(card);
            annihilationUIController.Refresh();
        }

        public override int IndexOf(GameCard card) => cards.IndexOf(card);
    }
}