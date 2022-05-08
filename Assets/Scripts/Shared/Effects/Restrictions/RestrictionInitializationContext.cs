using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Effects.Restrictions
{
    /// <summary>
    /// An object to hold all the parameters required to initialize any restriction/restriction elemnt
    /// </summary>
    public struct RestrictionInitializationContext
    {
        public readonly Game game;
        public readonly GameCard source;

        public RestrictionInitializationContext(Game game, GameCard source)
        {
            this.game = game;
            this.source = source;
        }
    }
}