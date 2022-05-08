using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Gets a single card from the current gamestate.
    /// </summary>
    public interface IGamestateCardIdentity
    {
        public GameCard CardFrom(Game game, ActivationContext context = default);
    }

    public class AnyGameCardIdentity : IGamestateCardIdentity
    {
        public IGamestateCardsIdentity gamestateCardsIdentity;

        public GameCard CardFrom(Game game, ActivationContext context = default)
            => gamestateCardsIdentity.GameCardsFrom(game, context).FirstOrDefault();
    }
}