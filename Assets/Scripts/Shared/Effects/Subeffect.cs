﻿using KompasCore.Cards;
using KompasCore.GameCore;

namespace KompasCore.Effects
{
    /// <summary>
    /// Not abstract because it's instantiated as part of loading subeffects
    /// </summary>
    [System.Serializable]
    public class Subeffect
    {
        public virtual Effect Effect { get; }
        public virtual Player Controller { get; }
        public virtual Game Game { get; }

        public int SubeffIndex { get; protected set; }

        public GameCard Source => Effect.Source;
        public ActivationContext Context => Effect.CurrActivationContext;

        /// <summary>
        /// Represents the type of subeffect this is
        /// </summary>
        public string subeffType;

        #region targeting indices
        /// <summary>
        /// The index in the Effect.targets array for which target this effect uses.
        /// If positive, just an index.
        /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
        /// </summary>
        public int targetIndex = -1;

        /// <summary>
        /// The index in the coords list that this subeffect uses.
        /// If positive, just an index.
        /// If negative, it's Count + targetIndex (aka that many back)
        /// </summary>
        public int spaceIndex = -1;

        /// <summary>
        /// The index of player. If 0, this, if 1, enemy. 
        /// </summary>
        public int playerIndex = -1;
        #endregion targeting indices

        #region effect x
        /// <summary>
        /// If the effect uses X, this is the multiplier to X. Default: 0
        /// </summary>
        public int xMultiplier = 0;

        /// <summary>
        /// If the effect uses X, this is the divisor to X. Default: 1
        /// </summary>
        public int xDivisor = 1;

        /// <summary>
        /// If the effect uses X, this is the modifier to X. Default: 0
        /// </summary>
        public int xModifier = 0;

        /// <summary>
        /// If the effect uses X, this is the adjusted value of X
        /// </summary>
        public int Count => (Effect.X * xMultiplier / xDivisor) + xModifier;
        #endregion effect x

        public GameCard Target => Effect.GetTarget(targetIndex);
        public Space Space => Effect.GetSpace(spaceIndex);
        public Player Player => Effect.GetPlayer(playerIndex);

        public bool RemoveTarget()
        {
            if (Target == null) return false;

            Effect.RemoveTarget(Target);
            return true;
        }
    }
}