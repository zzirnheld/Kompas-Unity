using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Identifies a collection of cards, based on the current gamestate
    /// </summary>
    public interface IGamestateCardsIdentity
    {
        public ICollection<GameCard> GameCardsFrom(Game game, ActivationContext context = default);
    }

    public class AllGameCardsIdentity
    {
        public CardRestriction cardRestriction;

        public ICollection<GameCard> GameCardsFrom(Game game, ActivationContext context = default)
        {
            return game.Cards.Where(c => cardRestriction.IsValidCard(c, context)).ToArray();
        }
    }
}