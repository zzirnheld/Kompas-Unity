using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class AnnihilationController : MonoBehaviour
    {
        public Game game;

        public List<GameCard> Cards { get; } = new List<GameCard>();

        public virtual bool Annihilate(GameCard card, IStackable stackSrc = null)
        {
            if(!card.Remove(stackSrc)) return false;
            Cards.Add(card);
            card.Location = CardLocation.Annihilation;
            return true;
        }

        public virtual bool Remove(GameCard card)
        {
            if (!Cards.Contains(card)) return false;

            Cards.Remove(card);
            return true;
        }
    }
}