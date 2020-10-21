using KompasCore.Cards;
using KompasCore.GameCore;

namespace KompasCore.Effects
{
    /// <summary>
    /// Not abstract because it's instantiated as part of loading subeffects
    /// </summary>
    [System.Serializable]
    public class Subeffect
    {
        #region subeffect type constants
        //TODO deprecate change nesw
        //misc
        public const string EndTurn = "End Turn";
        public const string Attack = "Attack";
        public const string ChangeLeyload = "Change Leyload";
        #endregion subeffect type constants

        public virtual Effect Effect { get; }
        public virtual Player Controller { get; }
        public virtual Game Game { get; }

        public int SubeffIndex { get; protected set; }

        public GameCard Source => Effect.Source;

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
        public int playerIndex = 0;

        /// <summary>
        /// Represents the type of subeffect this is
        /// </summary>
        public string subeffType;

        /// <summary>
        /// If the effect uses X, this is the multiplier to X
        /// </summary>
        public int xMultiplier = 0;

        /// <summary>
        /// If the effect uses X, this is the divisor to X
        /// </summary>
        public int xDivisor = 1;

        /// <summary>
        /// If the effect uses X, this is the modifier to X
        /// </summary>
        public int xModifier = 0;

        /// <summary>
        /// If the effect uses X, this is the adjusted value of X
        /// </summary>
        public int Count => (Effect.X * xMultiplier / xDivisor) + xModifier;

        public GameCard Target => Effect.GetTarget(targetIndex);
        public (int x, int y) Space => Effect.GetSpace(spaceIndex);
        public Player Player => Game.Players[(Controller.index + playerIndex) % Game.Players.Length];

        public bool RemoveTarget()
        {
            if (Target == null) return false;

            Effect.RemoveTarget(Target);
            return true;
        }
    }
}